using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Controllers
{
    public class CoreController : ControllerBase
    {
        #region Properties

        public string UserId
        {
            get
            {
                try
                {
                    return User.FindFirst("id").Value;
                }
                catch
                {
                    return "";
                }
            }
        }

        #endregion
    }
}
