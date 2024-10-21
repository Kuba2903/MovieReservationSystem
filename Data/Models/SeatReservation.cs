using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class SeatReservation
{
    public string Sector { get; set; } = null!;

    public string Seat { get; set; } = null!;

    public int ShowTimeId { get; set; }

    public virtual ShowTime ShowTime { get; set; } = null!;
}
