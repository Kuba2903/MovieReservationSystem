using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Type { get; set; } = null!;

        public double Price { get; set; }

        public ICollection<SeatReservation> Reservations { get; set; } = new List<SeatReservation>();
    }
}
