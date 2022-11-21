using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models.Users

{
    public class RegisterUserViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
    }
}
