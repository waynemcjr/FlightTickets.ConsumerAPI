using System.Text;
using System.Text.Json;
using FlightTickets.ConsumerAPI.Repository.Interface;
using FlightTickets.ConsumerAPI.Services.Interfaces;
using FlightTickets.Models.Models;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FlightTickets.ConsumerAPI.Services
{
    public class ConsumerService : IConsumerServices
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConsumerRepository _consumerRepository;

        public ConsumerService(ILogger<ConsumerService> logger, IConsumerRepository consumerRepository)
        {
            _logger = logger;
            _consumerRepository = consumerRepository;
        }

        public async Task GetTicketsFromQueuesAsync()
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: "TicketsApproved", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();

                    var message = Encoding.UTF8.GetString(body);

                    var ticket = JsonSerializer.Deserialize<Ticket>(message);

                    await SaveApprovedTicketsToCollectionAsync(ticket);
                };

                await channel.BasicConsumeAsync(queue: "TicketsApproved", autoAck: true, consumer: consumer);



                await channel.QueueDeclareAsync(queue: "TicketsDenied", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();

                    var message = Encoding.UTF8.GetString(body);

                    var ticket = JsonSerializer.Deserialize<Ticket>(message);

                    await SaveDeniedTicketsToCollectionAsync(ticket);
                };

                await channel.BasicConsumeAsync(queue: "TicketsDenied", autoAck: true, consumer: consumer);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error... " + ex.Message);
            }
        }

        public async Task SaveApprovedTicketsToCollectionAsync(Ticket ticket)
        {
            try
            {
                await _consumerRepository.SaveApprovedTicketsAsync(ticket);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error... " + ex.Message);
            }
        }

        public async Task SaveDeniedTicketsToCollectionAsync(Ticket ticket)
        {
            try
            {
                await _consumerRepository.SaveDeniedTicketsAsync(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error... " + ex.Message);
            }
        }
    }
}
