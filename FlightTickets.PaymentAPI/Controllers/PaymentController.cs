using FlightTickets.PaymentAPI.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightTickets.PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                _logger.LogInformation("Processando fila de passagens...");
                await _paymentService.GetTicketsFromQueueAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("Erro no processamento...");
                return Problem("Error: " + ex.Message);
            }
        }
    }
}
