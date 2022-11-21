using System;

namespace VetManage.Web.Data.Entities
{
    public interface IIsUser
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        string Address{ get; set; }

        string MobileNumber { get; set; }

        Guid ImageId { get; set; }

        string Gender { get; set; }

        DateTime DateOfBirth { get; set; }
    }
}
