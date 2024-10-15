using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly SalesApiDbContext _context;
        private readonly IMapper _mapper;

        public PaymentService(SalesApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllAsync()
        {
            var payments = await _context.Payments.ToListAsync();
            return _mapper.Map<IEnumerable<PaymentDTO>>(payments);
        }

        public async Task<PaymentDTO> GetByIdAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            return _mapper.Map<PaymentDTO>(payment);
        }

        public async Task<PaymentDTO> CreateAsync(PaymentDTO paymentDto)
        {
            var payment = _mapper.Map<Payment>(paymentDto);
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return _mapper.Map<PaymentDTO>(payment);
        }

        public async Task<PaymentDTO> UpdateAsync(PaymentDTO paymentDto)
        {
            var payment = await _context.Payments.FindAsync(paymentDto.PaymentId);
            if (payment == null) return null;

            _mapper.Map(paymentDto, payment);
            await _context.SaveChangesAsync();
            return _mapper.Map<PaymentDTO>(payment);
        }

        public async Task DeleteAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }
    }

}
