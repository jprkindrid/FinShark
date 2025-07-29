namespace api.Models
{
    public class Comment
    {
        public  int Id { get; set; }
        public  string Title { get; set; } = null!;
        public  string Content { get; set; } = null!;
        public  DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public  int? StockId { get; set; }
        public Stock? Stock { get; set; }
    }
}
