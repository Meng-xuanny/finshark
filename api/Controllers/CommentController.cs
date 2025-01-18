using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController:ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo=commentRepo;
            _stockRepo=stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() { 
            var comments = await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(c => c.ToCommentDto()).ToList();
            return Ok(commentDto);
        }
        
        [HttpGet("{id:int}")]
        
        public async Task<IActionResult> GetById([FromRoute]int id){
            var comment=await _commentRepo.GetByIdAsync(id);
            if (comment==null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute]int stockId, CreateCommentDto commentDto)
        {
            if(!await _stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            var commentModel=commentDto.ToCommentFromCreate(stockId);
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById),new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel==null)
            {
                return NotFound("Comment does not exist");
            }
            return Ok(commentModel);
        }
    }
};