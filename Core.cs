using Microsoft.Extensions.Logging;
using FluentScheduler;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Microsoft.DependencyInjection;
using PhoneNumbers;
using Mel.Live.Data;
using System.Reflection;
using Microsoft.Extensions.Configuration.UserSecrets;
using Mel.Live.Extensions.Configuration;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using Mel.Live.Models.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MongoDbGenericRepository.Models;
using System.Linq.Expressions;
using Mel.Live.Models.Entities.BackOffice;

namespace Mel.Live
{
    public static class Core
    {
        #region Properties
        public static Logger Log { get; } = LogManager.GetCurrentClassLogger();
        public static string[] StartupArguments { get; set; } = new string[0];
        public static IUnityContainer Container { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }
        public static PhoneNumberUtil Phone { get; } = PhoneNumberUtil.GetInstance();
        public static MongoDataContext DataContext => MongoDataContext.Current;

        #region Solids

        #region Names
        public static string PRODUCT_NAME = "Mel API";
        public static string PRODUCT_VERSION = "v0.1";
        public static string FULL_PRODUCT_NAME => $"{Core.PRODUCT_NAME} {Core.PRODUCT_VERSION}";
        public static string AUTHOR = "Prince Owen";
        public static string COMPANY = "Dev-Lynx Technologies";
        public const string CONSOLE_LOG_NAME = "console-debugger";
        public const string LOG_LAYOUT = "${longdate}|${uppercase:${level}}| ${message} ${exception:format=tostring";
        public const string FULL_LOG_LAYOUT = "${longdate} | ${logger}\n${message} ${exception:format=tostring}\n";
        public static readonly string ERROR_LOG_NAME = $"Errors_{DateTime.Now.ToString("MM-yyyy")}";
        public static readonly string RUNTIME_LOG_NAME = $"Runtime_{DateTime.Now.ToString("MM-yyyy")}";
        public const string REGION = "NG";

        public const string MelEnvKey = "MEL_LIVE_KEY";

        public const string CLOUDINARY_BASE_FOLDER = "Mel";
        #endregion

        #region Directories
        public static readonly string BASE_DIR = Directory.GetCurrentDirectory();
        public static readonly string WORK_DIR = Path.Combine(BASE_DIR, "App");
        public static readonly string DATA_DIR = Path.Combine(WORK_DIR, "Data");
        public static readonly string RESOURCES_DIR = Path.Combine(DATA_DIR, "Resources");
        public static readonly string INDEX_DIR = Path.Combine(WORK_DIR, "Indexes");
        public readonly static string LOG_DIR = Path.Combine(WORK_DIR, "Logs");
        #endregion

        #region Paths
        public static string RUNTIME_LOG_PATH => Path.Combine(LOG_DIR, RUNTIME_LOG_NAME + ".log");
        public static string ERROR_LOG_PATH => Path.Combine(LOG_DIR, ERROR_LOG_NAME + ".log");
        #endregion

        #region Routes
        public const string NGROK_SERVER = "http://c9dae43d.ngrok.io";
        public static string ONLINE_BASE_ADDRESS { get; set; }
        public const string DOCS_ROUTE = "/api/docs";

        public static class EmailTemplates
        {
            public static string Verification { get; } = Path.Combine(RESOURCES_DIR, "Templates\\verification.html");
        }
        #endregion

        #region JWT Claim Identifiers
        public const string JWT_CLAIM_ID = "id";
        public const string JWT_CLAIM_ROL = "rol";
        public const string JWT_CLAIM_ROLES = "roles";
        public const string JWT_CLAIM_VERIFIED = "ver";
        #endregion

        #region JWT Claims
        public const string JWT_CLAIM_API_USER = "api_user";
        public const string JWT_CLAIM_API_ACCESS = "api_access";
        #endregion

        #endregion

        #endregion

        #region Constructors
        static Core()
        {
            CreateDirectories(WORK_DIR, DATA_DIR, LOG_DIR);

            JobManager.Initialize(new Registry());

            ConfigureLogger();

            Core.Log.Info("\n\n");
            Core.Log.Info($"*** Application Started ***\nWelcome to the {PRODUCT_NAME}\nBuilt by {AUTHOR}" +
                $"\nUnder the supervision of {COMPANY}\nCopyright 2019.\nAll rights reserved.\n\n");
        }
        #endregion

        #region Methods

        #region Helpers
        /// <summary>
        /// Easy and safe way to create multiple directories. 
        /// </summary>
        /// <param name="directories">The set of directories to create</param>
        public static void CreateDirectories(params string[] directories)
        {
            if (directories == null || directories.Length <= 0) return;

            foreach (var directory in directories)
                try
                {
                    if (Directory.Exists(directory)) continue;

                    Directory.CreateDirectory(directory);
                    Log.Info("A new directory has been created ({0})", directory);
                }
                catch (Exception e)
                {
                    Log.Error("Error while creating directory {0} - {1}", directory, e);
                }
        }

        public static void ClearDirectory(string directory, bool removeDirectory = false)
        {
            if (string.IsNullOrWhiteSpace(directory)) return;

            foreach (var d in Directory.EnumerateDirectories(directory))
                ClearDirectory(d, true);

            foreach (var file in Directory.EnumerateFiles(directory, "*"))
                try { File.Delete(file); }
                catch (Exception e) { Log.Error("Failed to delete {0}\n", file, e); }

            if (removeDirectory)
                try { Directory.Delete(directory); }
                catch (Exception ex) { Log.Error("An error occured while attempting to remove a directory ({0})\n{1}", directory, ex); }
        }
        #endregion

        static void ConfigureLogger()
        {
            var config = new LoggingConfiguration();

#if DEBUG
            var debugConsole = new ColoredConsoleTarget()
            {
                Name = Core.CONSOLE_LOG_NAME,
                Layout = Core.FULL_LOG_LAYOUT,
                Header = $"{PRODUCT_NAME} Debugger"
            };

            var debugRule = new LoggingRule("*", NLog.LogLevel.Debug, debugConsole);
            config.LoggingRules.Add(debugRule);
#endif

            var errorFileTarget = new FileTarget()
            {
                Name = Core.ERROR_LOG_NAME,
                FileName = Core.ERROR_LOG_PATH,
                Layout = Core.LOG_LAYOUT
            };

            config.AddTarget(errorFileTarget);

            var errorRule = new LoggingRule("*", NLog.LogLevel.Error, errorFileTarget);
            config.LoggingRules.Add(errorRule);

            var runtimeFileTarget = new FileTarget()
            {
                Name = Core.RUNTIME_LOG_NAME,
                FileName = Core.RUNTIME_LOG_PATH,
                Layout = Core.LOG_LAYOUT
            };
            config.AddTarget(runtimeFileTarget);

            var runtimeRule = new LoggingRule("*", NLog.LogLevel.Trace, runtimeFileTarget);
            config.LoggingRules.Add(runtimeRule);



            LogManager.Configuration = config;

            LogManager.ReconfigExistingLoggers();

            DateTime oneMonthLater = DateTime.Now.AddMonths(1);
            DateTime nextMonth = new DateTime(oneMonthLater.Year, oneMonthLater.Month, 1);

            JobManager.AddJob(() =>
            {
                Core.Log.Debug("*** Monthly Session Ended ***");
                ConfigureLogger();
            }, s => s.ToRunOnceAt(nextMonth));
        }

        public static IWebHost Startup(string[] args)
        {
            StartupArguments = args;

            if (StartupArguments.Contains("--deploy-secrets"))
            {
                DeploySecrets();
                Environment.Exit(0);
                return null;
            }

            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args).Build();

            if (string.IsNullOrWhiteSpace(configuration["urls"]))
                configuration["urls"] = "http://0.0.0.0:7000";

            IWebHost host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel().UseUrls(configuration["urls"])
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration).UseIISIntegration()
                .ConfigureLogging((h, builder) =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                }).UseNLog()
                .UseUnityServiceProvider()
                .UseStartup<Startup>().Build();

            Core.Log.Debug("Successfully created startup class");

            if (StartupArguments.Contains("--reset-data"))
            {
                var env = Container.Resolve<IHostingEnvironment>();
                if (env.IsProduction())
                    Log.Warn("Manual Data Manipulation is not allowed in a production environment.");
                else ResetData();
            }

            InitializeData().Wait();
            return host;
        }


        #region Tasks
        static void DeploySecrets()
        {
            var attribute = Assembly.GetEntryAssembly().GetCustomAttribute<UserSecretsIdAttribute>();

            if (attribute == null)
            {
                Core.Log.Debug("Failed to get UserSecretsIdAttribute");
                return;
            }

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                $@"Microsoft\UserSecrets\{attribute.UserSecretsId}\secrets.json");

            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open))
                {
                    string protectedSecrets = ProtectedConfigurationProvider.Protect(stream, Environment.GetEnvironmentVariable(Core.MelEnvKey));
                    File.WriteAllText(Path.Combine(BASE_DIR, "appsettings.secrets.json"), protectedSecrets);

                    Core.Log.Debug("User-Secrets have successfully been protected");
                }
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while exporting protecting user secrets\n{ex}");
            }
        }

        static void ResetData()
        {
            Console.Write("You are about to refresh all the data in this environment. This action cannot be reversed. Are you sure you would like to continue? (Yes/No) > ");
            string reply = Console.ReadLine().ToLower();

            while (reply != "yes" && reply != "no" && reply != "y" && reply != "n")
            {
                Core.Log.Debug("Please enter a valid answer (Yes/No) > ");
                reply = Console.ReadLine().ToLower();
            }

            if (reply != "yes" && reply != "y")
                return;

            try
            {
                DataContext.Client.DropDatabase(DataContext.Database.DatabaseNamespace.DatabaseName);
                Core.Log.Debug("Successfully refreshed the database.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while refreshing a mongoDB database\n{ex}");
            }
        }

        static async Task InitializeData()
        {
            var roleManager = Container.Resolve<RoleManager<UserRole>>();
            var userManager = Container.Resolve<UserManager<User>>();
            var dataContext = Container.Resolve<MongoDataContext>();

            if (dataContext.Company == null)
            {
                Company company = new Company()
                {
                    Name = "Mel Endless",
                    Email = "melendless@gmail.com"
                };

                await dataContext.Store.AddOneAsync(company);
            }

            string[] roles = Enum.GetValues(typeof(UserRoles)).
                OfType<UserRoles>().Select(u => u.ToString()).ToArray();

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role)) continue;
                await roleManager.CreateAsync(new UserRole(role));
            }

            InitializeCollection<PaymentChannel, int>("paymentChannels.json").Wait();
            InitializeCollection<UserRank, int>("userRanks.json").Wait();

            if (await userManager.FindByEmailAsync("admin@mel.com") == null)
            {
                User adminAccount = new User()
                {
                    Email = "admin@mel.com",
                    UserName = "admin",
                    FirstName = "Admin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                await userManager.CreateAsync(adminAccount);
                await userManager.AddToRoleAsync(adminAccount, UserRoles.Administrator.ToString());
                await userManager.AddPasswordAsync(adminAccount, "Mel@123");
                await adminAccount.InitializeAsync();
            }
        }
        #endregion

        static async Task<bool> InitializeCollection<TDoc>(string fileName) where TDoc : Document
            => await InitializeCollection<TDoc, Guid>(fileName);

        static async Task<bool> InitializeCollection<TDoc, TKey>(string fileName) where TKey : IEquatable<TKey> where TDoc : IDocument<TKey>
        {
            string path = Path.Combine(DATA_DIR, fileName);
            string json = await File.ReadAllTextAsync(path);
            var collection = JsonConvert.DeserializeObject<List<TDoc>>(json);

            foreach (var document in collection)
            {
                bool exists = await DataContext.Store.AnyAsync<TDoc, TKey>(d => d.Id.Equals(document.Id));
                if (!exists) await DataContext.Store.AddOneAsync<TDoc, TKey>(document);
            }

            return true;
        }

        public static void ConfigureCoreServices(IUnityContainer container)
        {
            Container = container;
        }
        #endregion
    }
}
