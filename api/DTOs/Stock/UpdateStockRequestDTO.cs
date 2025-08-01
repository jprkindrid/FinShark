using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.DTOs.Stock
{
    public class UpdateStockRequestDTO
    {

        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string Symbol { get; set; } = null!;
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string CompanyName { get; set; } = null!;
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        [Range(1, 10000000000000)]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.001, 100)]
        public decimal LastDiv { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Industry cannot be over 10 characters")]
        public string Industry { get; set; } = null!;
        [Range(1, 5000000000000)]
        public long MarketCap { get; set; }
    }
}
