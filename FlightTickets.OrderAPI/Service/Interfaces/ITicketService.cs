using FlightTickets.Models.DTOs;

namespace FlightTickets.OrderAPI.Service.Interfaces
{
    public interface ITicketService
    {
        Task<TicketResponseDTO> CreateTicketAsync(TicketRequestDTO ticketRequest);
    }
}
