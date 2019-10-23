using Mel.Live.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringRange(Range = new[] { "Male", "Female" })]
        public string Gender { get; set; }

        [Required]
        public string StateOfOrigin { get; set; }

        public ReferrerViewModel Referrer { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// API Access Token Wrapper
    /// </summary>
    public class AccessTokenModel
    {
        /// <summary>
        /// JWT Access Token for user account.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }
    }
}
