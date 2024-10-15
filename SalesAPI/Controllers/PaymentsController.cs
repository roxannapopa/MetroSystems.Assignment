using Microsoft.AspNetCore.Mvc;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;

namespace SalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetAll()
        {
            try
            {
                var payments = await _paymentService.GetAllAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all payments.");
                return StatusCode(500, "Internal server error while retrieving payments.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var payment = await _paymentService.GetByIdAsync(id);
                if (payment == null) return NotFound();
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payment with ID {id}.");
                return StatusCode(500, "Internal server error while retrieving the payment.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDTO>> Create(PaymentDTO paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPayment = await _paymentService.CreateAsync(paymentDto);
                return CreatedAtAction(nameof(GetById), new { id = createdPayment.PaymentId }, createdPayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new payment.");
                return StatusCode(500, "Internal server error while creating the payment.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PaymentDTO paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentDto.PaymentId) return BadRequest();

            try
            {
                var updatedPayment = await _paymentService.UpdateAsync(paymentDto);
                if (updatedPayment == null) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating payment with ID {id}.");
                return StatusCode(500, "Internal server error while updating the payment.");
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
                var payment = await _paymentService.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                await _paymentService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting payment with ID {id}.");
                return StatusCode(500, "Internal server error while deleting the payment.");
            }
        }
    }
}
