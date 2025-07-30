using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepo;

        public CommentController(ICommentRepository commentRepo)
        {
            this.commentRepo = commentRepo;
        }
        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var comments = await commentRepo.GetAllAsync();

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
            
        }
    }
}
