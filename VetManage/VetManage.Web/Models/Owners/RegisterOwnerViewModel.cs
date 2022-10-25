using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models.Owners
{
    public class RegisterOwnerViewModel
    {
        public OwnerViewModel OwnerViewModel { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }
    }
}
