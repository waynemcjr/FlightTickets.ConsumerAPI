using FlightTickets.Models.Models;

namespace FlightTickets.PaymentAPI.Service.Interface
{
    public interface IPaymentService
    {
        Task GetTicketsFromQueueAsync();
    }
}
