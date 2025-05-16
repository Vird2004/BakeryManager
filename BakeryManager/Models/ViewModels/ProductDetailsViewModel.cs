using System.ComponentModel.DataAnnotations;

namespace BakeryManager.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetails { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập đánh giá sản phẩm.")]
        public string Comment { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập tên người đánh giá.")]
        public string Name { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập email người đánh giá.")]
        public string Email { get; set; }
    }
}
