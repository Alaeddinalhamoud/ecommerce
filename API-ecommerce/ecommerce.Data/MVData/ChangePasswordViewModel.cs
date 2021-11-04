using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data.MVData
{
  public  class ChangePasswordViewModel
    {
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage ="The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
