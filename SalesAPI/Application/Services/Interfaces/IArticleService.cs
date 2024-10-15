using SalesAPI.Application.DTOs;

namespace SalesAPI.Application.Services.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDTO>> GetAllAsync();
        Task<ArticleDTO> GetByIdAsync(int articleId);
        Task<ArticleDTO> CreateAsync(ArticleDTO articleDto);
        Task<ArticleDTO> UpdateAsync(ArticleDTO articleDto);
        Task DeleteAsync(int articleId);
    }
}
