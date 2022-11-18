using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models.Account
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
