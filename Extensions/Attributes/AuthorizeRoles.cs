using Mel.Live.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Extensions.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        public AuthorizeRolesAttribute(params UserRoles[] roles)
        {
            Roles = string.Join(",", roles.ToDescriptions());
        }
    }
}
