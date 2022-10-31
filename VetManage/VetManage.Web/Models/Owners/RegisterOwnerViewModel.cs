using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models.Owners
{
    public class RegisterOwnerViewModel
    {
        public OwnerViewModel OwnerViewModel { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
    }
}
