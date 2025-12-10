using FlightTickets.ConsumerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightTickets.ConsumerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private readonly ILogger<ConsumerController> _logger;
        private readonly IConsumerServices _consumerService;

        public ConsumerController(ILogger<ConsumerController> logger, IConsumerServices consumerService)
        {
            _logger = logger;
            _consumerService = consumerService;
        }

        [HttpPost]
        public async Task<IActionResult> TicketSaveDatabase()
        {
            try
            {
                _logger.LogInformation("Reading tickets from queues...");
                await _consumerService.GetTicketsFromQueuesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("Shit happens... " + ex.Message);
                throw;
            }
        }
    }
}
