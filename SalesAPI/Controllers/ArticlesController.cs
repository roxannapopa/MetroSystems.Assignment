using Microsoft.AspNetCore.Mvc;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;

namespace SalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IArticleService articleService, ILogger<ArticlesController> logger)
        {
            _articleService = articleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDTO>>> GetAll()
        {
            try
            {
                var articles = await _articleService.GetAllAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all articles.");
                return StatusCode(500, "Internal server error while retrieving articles.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var article = await _articleService.GetByIdAsync(id);
                if (article == null) return NotFound();
                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving article with ID {id}.");
                return StatusCode(500, "Internal server error while retrieving the article.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDTO>> Create(ArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdArticle = await _articleService.CreateAsync(articleDto);
                return CreatedAtAction(nameof(GetById), new { id = createdArticle.ArticleId }, createdArticle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new article.");
                return StatusCode(500, "Internal server error while creating the article.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != articleDto.ArticleId) return BadRequest();

            try
            {
                var updatedArticle = await _articleService.UpdateAsync(articleDto);
                if (updatedArticle == null) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating article with ID {id}.");
                return StatusCode(500, "Internal server error while updating the article.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var article = await _articleService.GetByIdAsync(id);
                if (article == null)
                {
                    return NotFound($"Article with ID {id} not found.");
                }

                await _articleService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting article with ID {id}.");
                return StatusCode(500, "Internal server error while deleting the article.");
            }
        }
    }
}
