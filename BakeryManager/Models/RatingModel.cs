using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakeryManager.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        
        public long ProductId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập đánh giá sản phẩm.")]
        public string Comment { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập tên người đánh giá.")]
        public string Name { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Yêu cầu nhập email người đánh giá.")]
        public string Email { get; set; }

        public string Stars { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
