using System.Collections.Generic;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Account
{
    public class AccountViewModel
    {
        public ICollection<User> Users { get; set; }

        public EditUserViewModel EditUser { get; set; }

        public RegisterNewUserViewModel RegisterNewUser { get; set; }
    }
}
