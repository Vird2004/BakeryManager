using System.ComponentModel.DataAnnotations;

namespace BakeryManager.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập tên sản phẩm.")]
        public string Name { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm.")]
        public string Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }
        public string Slug { get; set; }

        public string Image { get; set; }
        public string Status { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
    }
}
