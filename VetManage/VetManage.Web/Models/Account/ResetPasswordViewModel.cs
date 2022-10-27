using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
