using api.DTOs.Comment;
using api.DTOs.Stock;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepo;
        private readonly IStockRepository stockRepo;
        private readonly UserManager<AppUser> userManager;
        private readonly IFMPService fmpService;
        private readonly ILogger<CommentController> logger;

        public CommentController(
            ICommentRepository commentRepo,
            IStockRepository stockRepo,
            UserManager<AppUser> userManager,
            IFMPService fmpService,
            ILogger<CommentController> logger)
        {
            this.commentRepo = commentRepo;
            this.stockRepo = stockRepo;
            this.userManager = userManager;
            this.fmpService = fmpService;
            this.logger = logger;
        }
        [HttpGet]
        [Authorize]


        public async Task<IActionResult> GetAll([FromQuery] CommentQueryObject queryObject)
        {

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var comments = await commentRepo.GetAllAsync(queryObject);

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);

        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await commentRepo.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentDto());
        }
        [HttpPost("{symbol:alpha}")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDTO commentDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stock = await stockRepo.GetBySymbolAsync(symbol);
            if (stock == null)
            {
                stock = await fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exist");
                }
                await stockRepo.CreateAsync(stock);
            }
            
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var appUser = await userManager.FindByNameAsync(username);
            if (appUser == null)
                return Unauthorized("User not found");

            var commentModel = commentDto.ToCommentFromCreate(stock.Id);
            commentModel.AppUserID = appUser.Id;
            await commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetByID), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
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
        [Authorize]
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
