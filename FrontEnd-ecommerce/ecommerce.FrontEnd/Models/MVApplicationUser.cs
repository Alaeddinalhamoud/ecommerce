using ecommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.FrontEnd.Models
{
    public class MVApplicationUser : IdentityUser
    {
        public string name { get; set; }
        [DataType(DataType.Date)]
        [Required, Remote(action: "Is18YearsOld", controller: "Account")]
        public DateTime dOB { get; set; }
        public Genders gender { get; set; }
        [NotMapped]
        public bool isVendor { get; set; }
        [NotMapped]
        public bool isAdmin { get; set; }
        public bool isBlocked { get; set; }
        public string modifiedBy { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime lastLogin { get; set; }
    }
}
