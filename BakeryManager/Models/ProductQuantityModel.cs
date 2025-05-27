using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakeryManager.Models
{
    public class ProductQuantityModel
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "Yêu cầu nhập số lượng sản phẩm.")]
        
        public long Quantity { get; set; }

        public long ProductId { get; set; }
        
        public DateTime Date { get; set; }
       
    }
}
