using BakeryManager.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakeryManager.Models
{
    public class ContactModel
    {
        [Key]

        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập tiêu đề.")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(10000 ,ErrorMessage = "Yêu cầu nhập địa chỉ.")]
        public string Map { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập email liên hệ.")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập số điện thoại.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập thông tin.")]
        public string Description { get; set; }
        public string LogoImage { get; set; }

        [NotMapped]
        [FileExtention]
        public IFormFile? ImageUpload { get; set; }
    }
}
