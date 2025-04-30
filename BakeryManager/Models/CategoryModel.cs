using System.ComponentModel.DataAnnotations;

namespace BakeryManager.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập tên danh mục.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập mô tả danh mục.")]
        public string Description { get; set; }

        public string Slug { get; set; }
        public string Status { get; set; }
    }
}
