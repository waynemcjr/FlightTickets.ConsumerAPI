using System.Text;
using System.Text.Json;
using FlightTickets.Models.Models;
using FlightTickets.PaymentAPI.Service.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FlightTickets.PaymentAPI.Service
{
    public class PaymentService : IPaymentService
    {
        public async Task GetTicketsFromQueueAsync()
        {
            try
            {
                var factory = new ConnectionFactory{ HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: "TicketOrder", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();

                    var message = Encoding.UTF8.GetString(body);

                    var ticket = JsonSerializer.Deserialize<Ticket>(message);

                    await ValidadePaymentTicket(ticket);
                };

                await channel.BasicConsumeAsync(queue: "TicketOrder", autoAck: true, consumer: consumer);

            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message);
            }
        }

        private async Task ValidadePaymentTicket(Ticket ticket)
        {

            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            if (ticket.Price > 1000)
            {
                await channel.QueueDeclareAsync(queue: "TicketsApproved", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                var ticketString = JsonSerializer.Serialize(ticket);
                var body = Encoding.UTF8.GetBytes(ticketString);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "TicketsApproved", body: body);
            }
            else
            {
                await channel.QueueDeclareAsync(queue: "TicketsDenied", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                var ticketString = JsonSerializer.Serialize(ticket);
                var body = Encoding.UTF8.GetBytes(ticketString);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "TicketsDenied", body: body);
            }
        }
    }
}
