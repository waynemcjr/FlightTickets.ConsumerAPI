using MongoDB.Bson;

namespace FlightTickets.Models.Models
{
    public class Ticket
    {
        public ObjectId Id { get; private set; }
        public string PassengerName { get; private set; }
        public string FlightNumber { get; private set; }
        public string SeatNumber { get; private set; }
        public decimal Price { get; private set; }

        public Ticket(string passengerName, string flightNumber, string seatNumber, decimal price)
        {
            this.Id = ObjectId.GenerateNewId();
            PassengerName = passengerName;
            FlightNumber = flightNumber;
            SeatNumber = seatNumber;
            Price = price;
        }
    }
}
