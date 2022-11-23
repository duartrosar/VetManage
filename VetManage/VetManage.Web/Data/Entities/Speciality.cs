using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Speciality : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage="You must enter a Speciality Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must enter a Speciality Description")]
        public string Description { get; set; }

        public List<Treatment> Treatments { get; set; }
    }
}
