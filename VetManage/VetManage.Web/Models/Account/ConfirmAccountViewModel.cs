using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models.Account
{
    public class ConfirmAccountViewModel
    {
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string ConfirmationToken { get; set; }
        public string PasswordToken { get; set; }
    }
}
