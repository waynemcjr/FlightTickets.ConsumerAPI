using FlightTickets.ConsumerAPI.Data;
using FlightTickets.ConsumerAPI.Repository.Interface;
using FlightTickets.Models.Models;
using MongoDB.Driver;

namespace FlightTickets.ConsumerAPI.Repository
{
    public class ConsumerRepository : IConsumerRepository
    {
        private readonly ILogger<ConsumerRepository> _logger;
        private readonly IMongoCollection<Ticket> _collectionApproved;
        private readonly IMongoCollection<Ticket> _collectionDenied;

        public ConsumerRepository(ILogger<ConsumerRepository> logger, ConnectionDB connection)
        {
            _logger = logger;
            _collectionApproved = connection.GetMongoApprovedCollection();
            _collectionDenied = connection.GetMongoDeniedCollection();
        }

        public async Task SaveApprovedTicketsAsync(Ticket ticket)
        {
            try
            {
                _logger.LogInformation("Saving approved ticket!");
                await _collectionApproved.InsertOneAsync(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
            }
        }

        public async Task SaveDeniedTicketsAsync(Ticket ticket)
        {
            try
            {
                _logger.LogInformation("Saving denied ticket!");
                await _collectionDenied.InsertOneAsync(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
            }
        }
    }
}
