using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Unity;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Data;
using Mel.Live.Models.Entities;
using NLog;
using Mel.Live;
using Mel.Live.Services.Interfaces;

namespace Mel.Live.Services
{
    [AutoBuild]
    public class AuthService : IAuthService
    {
        #region Properties

        #region Options
        //public AuthSettings AuthSettings => OAuthSettings.Value;
        #endregion

        #region Services
        //[DeepDependency]
        //IOptions<AuthSettings> OAuthSettings { get; }

         /*
        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMongoRepository DataStore { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        IUnityContainer Container { get; }
        */
        #endregion

        #region Internals
        //ITokenGenerator Generator { get; set; }
        #endregion

        #endregion

        #region Methods

        public void SayHello()
        {
            Core.Log.Debug("Hello World");
        }

        #endregion
    }
}
