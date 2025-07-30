using api.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.DTOs.Stock
{
    public class StockDTO
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public decimal Price { get; set; }

        public decimal LastDiv { get; set; }
        public string Industry { get; set; } = null!;
        public long MarketCap { get; set; }
    }
}
