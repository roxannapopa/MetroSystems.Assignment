using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly SalesApiDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(SalesApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllAsync()
        {
            var customers = await _context.Customers.ToListAsync();
            return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> GetByIdAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CustomerDTO> CreateAsync(CustomerDTO customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CustomerDTO> UpdateAsync(CustomerDTO customerDto)
        {
            var customer = await _context.Customers.FindAsync(customerDto.CustomerId);
            if (customer == null) return null;

            _mapper.Map(customerDto, customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task DeleteAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }

}
