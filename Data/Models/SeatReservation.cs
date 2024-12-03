using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public partial class SeatReservation
{
    public string Sector { get; set; } = null!;

    public string Seat { get; set; } = null!;

    public int ShowTimeId { get; set; }

    public string? UserId { get; set; }
    public int? TicketId { get; set; }

    /// <summary>
    /// payment status of the reservation for example is it paid or not
    /// </summary>
    [MaxLength(150)]
    public string? PaymentStatus { get; set; }

    public virtual ShowTime ShowTime { get; set; } = null!;

    public virtual ApplicationUser? User { get; set; }

    public virtual Ticket Ticket { get; set; }
}
