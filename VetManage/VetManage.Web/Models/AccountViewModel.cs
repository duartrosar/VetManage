using System.Collections.Generic;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class AccountViewModel
    {
        public ICollection<User> Users { get; set; }

        public EditUserViewModel EditUser { get; set; }
    }
}
