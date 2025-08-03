using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.DTOs.Stock
{
    public class CreateStockRequestDTO
    {
        [Required]
        [MaxLength(4, ErrorMessage = "Symbol cannot be over 4 characters")]
        public string Symbol { get; set; } = null!;
        [MaxLength(30, ErrorMessage = "CompanyName cannot be over 30 characters")]
        public string CompanyName { get; set; } = null!;
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        [Range(1, 10000000000000)]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.001, 100)]
        public decimal LastDiv { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Industry cannot be over 30 characters")]
        public string Industry { get; set; } = null!;
        [Range(1, 5000000000000)]
        public long MarketCap { get; set; }
    }
}
