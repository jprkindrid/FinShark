namespace api.DTOs.Comment
{
    public class UpdateCommentRequestDTO
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
