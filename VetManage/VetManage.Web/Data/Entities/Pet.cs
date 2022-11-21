using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Pet : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage= "You must enter a Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage="You must enter a Species")]
        [MaxLength(50)]
        [Display(Name = "Species")]
        public string Type { get; set; }

        [Required(ErrorMessage="You must enter a Breed")]
        [MaxLength(50)]
        public string Breed { get; set; }

        [Required(ErrorMessage="You must enter a Weight")]
        public double Weight { get; set; }

        [Required(ErrorMessage="You must enter a Height")]
        public double Height { get; set; }

        [Required(ErrorMessage="You must enter a Length")]
        public double Length { get; set; }

        [Required(ErrorMessage="You must enter a Date Of Birth")]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage="You must enter a Gender")]
        [Display(Name = "Sex")]
        public string Gender { get; set; }

        public Owner Owner { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must select an owner")]
        public int OwnerId { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
             ? $"https://vetmanage.azurewebsites.net/images/noimage.png"
             : $"https://vetmanagestorage.blob.core.windows.net/pets/{ImageId}";
    }
}
