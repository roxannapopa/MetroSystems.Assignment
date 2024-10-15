using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly SalesApiDbContext _context;
        private readonly IMapper _mapper;

        public ArticleService(SalesApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllAsync()
        {
            var articles = await _context.Articles.ToListAsync();
            return _mapper.Map<IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<ArticleDTO> GetByIdAsync(int articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            return _mapper.Map<ArticleDTO>(article);
        }

        public async Task<ArticleDTO> CreateAsync(ArticleDTO articleDto)
        {
            var article = _mapper.Map<Article>(articleDto);
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return _mapper.Map<ArticleDTO>(article);
        }

        public async Task<ArticleDTO> UpdateAsync(ArticleDTO articleDto)
        {
            var article = await _context.Articles.FindAsync(articleDto.ArticleId);
            if (article == null) return null;

            _mapper.Map(articleDto, article);
            await _context.SaveChangesAsync();
            return _mapper.Map<ArticleDTO>(article);
        }

        public async Task DeleteAsync(int articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }
    }

}
