using BakeryManager.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakeryManager.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập tên slider.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập mô tả slider.")]
        public string Description { get; set; }
        public int? Status { get; set; }
        public string Image { get; set; }
        [NotMapped]
        [FileExtention]
        public IFormFile? ImageUpload { get; set; }
    }
}
