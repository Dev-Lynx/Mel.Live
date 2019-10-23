using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore;
using AutoMapper;
using CloudinaryDotNet;
using Mel.Live.API.Extensions.Logging;
using Mel.Live.Data;
using Mel.Live.Extensions;
using Mel.Live.Extensions.Configuration;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Mapping;
using Mel.Live.Models.Options;
using Mel.Live.Services;
using Mel.Live.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NSwag;
using NSwag.Generation.Processors.Security;
using PayStack.Net;
using Sieve.Services;
using Unity;

namespace Mel.Live
{
    public class Startup
    {
        #region Properties
        IConfiguration Configuration { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        #endregion

        #region Constructors
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.env.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false)
                .AddProtectedProvider("appsettings.secrets.json", optional: true, reloadOnChange: true,
                    cipher: Environment.GetEnvironmentVariable(Core.MelEnvKey));

            if (env.IsDevelopment()) builder.AddUserSecrets(Assembly.GetEntryAssembly(), true);

            Configuration = builder.Build();
        }
        #endregion

        #region Methods
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            services.AddAutoMapper(config => 
            {
                config.AddProfile<ViewModelToEntityProfile>();
                config.AddProfile<EntityToViewModelProfile>();
                config.AddProfile<BackOfficeViewModelToEntityProfile>();
                config.AddProfile<BackOfficeEntityToViewModelProfile>();
            }, Assembly.GetCallingAssembly());
            
            MongoDBOptions mongoOptions = Configuration.GetSection(MongoDBOptions.Key).Get<MongoDBOptions>();
            ConnectionString = mongoOptions.ConnectionString.BindTo(mongoOptions);
            DatabaseName = mongoOptions.DatabaseName;

            services.AddLogging();
            ConfigureOptions(services);
            ConfigureAuthentication(services);


            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
            
            services.AddOpenApiDocument(doc =>
            {
                doc.Title = Core.PRODUCT_NAME;
                doc.DocumentName = Core.FULL_PRODUCT_NAME;
                doc.Description = $"API Documentation for {Core.PRODUCT_NAME}";
                doc.Version = "v1";

                doc.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                doc.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }

        IServiceCollection ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions();

            var auth = Configuration.GetSection(AuthSettings.ConfigKey).Get<AuthSettings>();

            services.Configure<PaystackOptions>(Configuration.GetSection(PaystackOptions.ConfigKey));
            services.Configure<MongoDBOptions>(Configuration.GetSection(MongoDBOptions.Key));
            services.Configure<JwtIssuerOptions>(opt =>
            {
                var options = Configuration.GetSection(JwtIssuerOptions.ConfigKey).Get<JwtIssuerOptions>();
                options.SigningCredentials = new SigningCredentials(auth.SymmetricKey,SecurityAlgorithms.HmacSha512Signature);

                opt.Audience = options.Audience;
                opt.Issuer = options.Issuer;
                opt.SigningCredentials = options.SigningCredentials;
                opt.Subject = opt.Subject;
            });
            services.Configure<CloudinaryOptions>(Configuration.GetSection(CloudinaryOptions.ConfigKey));

            return services;
        }

        IApplicationBuilder ConfigureDocumentation(IApplicationBuilder app)
        {
            app.UseOpenApi(config =>
            {
                config.Path = Core.DOCS_ROUTE + "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUi3(config =>
            {
                config.Path = $"{Core.DOCS_ROUTE}/swagger";
                config.DocumentPath = Core.DOCS_ROUTE + "/swagger/{documentName}/swagger.json";
            });

            app.UseReDoc(config =>
            {
                config.Path = $"{Core.DOCS_ROUTE}/redoc";
                config.DocumentPath = Core.DOCS_ROUTE + $"/swagger/{Core.FULL_PRODUCT_NAME}/swagger.json";
            });
            return app;
        }

        IServiceCollection ConfigureAuthentication(IServiceCollection services)
        {
            services.AddCors();

            services.AddIdentity<User, UserRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;

                opt.User.RequireUniqueEmail = true;
            })
            .AddMongoDbStores<User, UserRole, Guid>(new MongoDataContext(ConnectionString, DatabaseName))
            .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                var options = Configuration.GetSection(JwtIssuerOptions.ConfigKey).Get<JwtIssuerOptions>();
                var auth = Configuration.GetSection(AuthSettings.ConfigKey).Get<AuthSettings>();

                configureOptions.ClaimsIssuer = options.Issuer;
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,

                    ValidateAudience = true,
                    ValidAudience = options.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = auth.SymmetricKey,

                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.SaveToken = true;
            });

            services.AddAuthorization();
            return services;
        }



        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
            //    app.UseHsts();
            }

            // app.UseHttpsRedirection();


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            ConfigureDocumentation(app);

            app.UseMvc();
            app.UseStaticFiles();

            app.UseSpaStaticFiles();
            app.UseSpa(spa => { });
        }

        #endregion

        public void ConfigureContainer(IUnityContainer container)
        {
            container.AddNewExtension<UnityFallbackProviderExtension>();
            container.AddNewExtension<AutomaticBuildExtension>();
            container.AddNewExtension<DeepDependencyExtension>();
            container.AddNewExtension<DeepMethodExtension>();
            container.RegisterInstance(new LoggerFactory().AddNLog());
            container.RegisterTransient(typeof(ILogger<>), typeof(LoggingAdapter<>));


            container.RegisterType<IUserStore<User>, MongoUserStore<User, UserRole, MongoDataContext, Guid>>();
            container.RegisterType<IRoleStore<UserRole>, MongoRoleStore<UserRole, MongoDataContext, Guid>>();

            container.RegisterFactory<IPayStackApi>(c => new PayStackApi(c.Resolve<IOptions<PaystackOptions>>().Value.SecretKey));
            container.RegisterTransient<IPaymentService, PaymentService>();

            container.RegisterFactory<MongoDataContext>(s => new MongoDataContext(ConnectionString, DatabaseName));
            container.RegisterScoped<ISieveProcessor, SieveProcessor>();

            container.RegisterControllers();

            Core.ConfigureCoreServices(container);


            container.RegisterScoped<IAuthService, AuthService>();
            container.RegisterScoped<IJwtFactory, JwtFactory>();
            container.RegisterFactory<Cloudinary>(c =>
            {
                var opt = container.Resolve<IOptions<CloudinaryOptions>>().Value;
                return new Cloudinary(new Account(opt.CloudName, opt.Key, opt.Secret));
            });
            container.RegisterType<IResourceManager, ResourceManager>();

            /*
            container.RegisterScoped<IEmailService, GoogleMailService>();
            container.RegisterScoped<ISmsService, SmsService>();
            container.RegisterFactory<PhoneNumberUtil>(c => PhoneNumberUtil.GetInstance());
            */
        }
       
    }
}

