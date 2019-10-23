using Mel.Live.Extensions.Attributes;
using Mel.Live.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels
{
    public class UserViewModel
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
        public UserRankViewModel Rank { get; set; }

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
        public decimal Balance { get; set; }

        public PhotoViewModel DisplayPicture { get; set; }

        public List<NodeViewModel> Nodes { get; set; } 

        public BasicStoreAccountViewModel StoreAccount { get; set; }
    }

    public class ReferrerViewModel
    {
        [Required]
        public Guid User { get; set; }
        [Required]
        public int Node { get; set; }
    }

    public class UserRankViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Badge { get; set; }
    }

    public class NodeViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Index { get; set; }
    }

    public class PasswordUpdateViewModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserTokenViewModel
    {
        public string Token { get; set; }
    }
}
