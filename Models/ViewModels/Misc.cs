using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels
{
    public class ReferenceViewModel
    {
        public string Id { get; set; }
    }

    public class EmailViewModel
    {
        public string To { get; set; }

        /// <summary>
        /// Subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body of the email. HTML is the preferred format.
        /// Should be less than 35MB.
        /// </summary>
        [Required]
        [DataType(DataType.Html)]
        public string Body { get; set; }
    }
}
