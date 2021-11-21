using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Auth.Quickstart.Models
{
    public class RegisterVM
    {
        public string FullName { get; set; }
        [EmailAddress, Required, Remote(action: "IsEmailnUse", controller: "Account")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
        [Required]
        public string Token { get; set; }
        [Required, DataType(DataType.DateTime), Remote(action: "Is18YearsOld", controller: "Account")]

        public DateTime DOB { get; set; }
        [Required, DataType(DataType.PhoneNumber)]
        public string phone { get; set; }
    }
}
