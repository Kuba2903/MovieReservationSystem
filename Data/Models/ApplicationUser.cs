﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();
    }
}
