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

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var comments = await commentRepo.GetAllAsync();

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await commentRepo.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentDto());
        }
        // FIXME: Big long error idk why this broke
        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDTO commentDto, IStockRepository stockRepo)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!await stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            await commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetByID), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDTO updateDTO)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var comment = await commentRepo.UpdateAsync(id, updateDTO.ToCommentFromUpdate());

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());  
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await commentRepo.DeleteAsync(id);
            if (commentModel == null)
            {
                return NotFound("Comment not found");
            }

            return NoContent();
        }

    }
}
