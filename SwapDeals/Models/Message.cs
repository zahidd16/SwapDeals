//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SwapDeals.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Message
    {
        public int MessageID { get; set; }
        public int UserID { get; set; }
        [Required]
        public string Message1 { get; set; }
    
        public virtual User User { get; set; }
    }
}
