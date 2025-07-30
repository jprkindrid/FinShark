using api.DTOs.Comment;
using api.DTOs.Stock;
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
        private readonly IStockRepository stockRepo;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            this.commentRepo = commentRepo;
            this.stockRepo = stockRepo;
        }
        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var comments = await commentRepo.GetAllAsync();

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute]int id)
        {
            var comment = await commentRepo.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentDto());
        }
        // FIXME: Big long error idk why this broke
        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute]int stockId, CreateCommentDTO commentDto, IStockRepository stockRepo)
        {
            if(!await stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            await commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetByID), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

    }
}
