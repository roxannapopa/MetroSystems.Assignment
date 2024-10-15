using AutoMapper;
using SalesAPI.Application.DTOs;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Domain.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Article, ArticleDTO>().ReverseMap();

            // Payment mapping
            CreateMap<Payment, PaymentDTO>().ReverseMap();

            // Transaction mapping with articles and payments
            CreateMap<TransactionDTO, Transaction>()
                .ForMember(dest => dest.TransactionArticles, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments.Select(p => new Payment
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod,
                    TransactionId = p.TransactionId
                    
                })))
                .ReverseMap()
                .ForMember(dest => dest.ArticleIds, opt => opt.MapFrom(src => src.TransactionArticles.Select(at => at.ArticleId)))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));

            // Mapping for ArticleTransaction
            CreateMap<int, ArticleTransaction>()
                .ForMember(at => at.ArticleId, opt => opt.MapFrom(src => src));
        }
    }

}
