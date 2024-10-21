using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class ShowTime
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public DateTime? ShowDate { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();
}
