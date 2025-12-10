namespace FlightTickets.ConsumerAPI.Data
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; }
        public string DataBaseName { get; set; }
        public string TicketApprovedCollection { get; set; }
        public string TicketDeniedCollection { get; set; }
    }
}
