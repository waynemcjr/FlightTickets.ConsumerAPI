using System.Text;
using System.Text.Json;
using FlightTickets.Models.DTOs;
using FlightTickets.Models.Models;
using FlightTickets.OrderAPI.Service.Interfaces;
using RabbitMQ.Client;

namespace FlightTickets.OrderAPI.Service
{
    public class TicketService : ITicketService
    {
        public async Task<TicketResponseDTO> CreateTicketAsync(TicketRequestDTO ticketRequest)
        {
            try
            {
                var newTicket = new Ticket
                (
                    ticketRequest.PassengerName,
                    ticketRequest.FlightNumber,
                    ticketRequest.SeatNumber,
                    ticketRequest.Price
                );

                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: "TicketOrder", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                var ticketString = JsonSerializer.Serialize(newTicket);

                var queueTicket = Encoding.UTF8.GetBytes(ticketString);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "TicketOrder", body: queueTicket);

                return new TicketResponseDTO
                {
                    Id = newTicket.Id.ToString(),
                    PassengerName = newTicket.PassengerName,
                    FlightNumber = newTicket.FlightNumber,
                    SeatNumber = newTicket.SeatNumber,
                    Price = ticketRequest.Price
                };

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured " + ex.Message);
            }
        }
    }
}
