using FlightTickets.Models.Models;

namespace FlightTickets.ConsumerAPI.Repository.Interface
{
    public interface IConsumerRepository
    {
        public Task SaveApprovedTicketsAsync(Ticket ticket);
        public Task SaveDeniedTicketsAsync(Ticket ticket);
    }
}
