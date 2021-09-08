using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SwapDeals.Models
{
    public class TempUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
        [Required]
        [MaxLength(15, ErrorMessage = "Maximum password length is 15")]
        [MinLength(5, ErrorMessage = "Minimum password length is 5")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }
    }
}