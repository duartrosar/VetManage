using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Pet : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Species")]
        public string Type { get; set; }

        [Required]
        [MaxLength(50)]
        public string Breed { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public double Height { get; set; }

        [Required]
        public double Length { get; set; }

        [Required]
        [Display(Name = "DOB")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public string Gender { get; set; }

        public Owner Owner { get; set; }

        public int OwnerId { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return $"https://localhost:44318/images/noimage.png";
                }

                return $"https://localhost:44318{ImageUrl.Substring(1)}";
            }
        }
    }
}
