using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Services.Implementations;

namespace WebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly StripePaymentService _paymentService;
        private readonly MovieReservationSystemContext _context;
        public PaymentController(MovieReservationSystemContext _context)
        {
            _paymentService = new StripePaymentService();
            this._context = _context;
        }

        [HttpGet]
        public IActionResult GetPaymentDetails(List<SeatReservation> reservations)
        {
            if (reservations == null || !reservations.Any())
            {
                return RedirectToAction("Reservations", "User"); 
            }

            var allReservations = _context.SeatReservations
                .Include(r => r.Ticket)
                .Include(r => r.ShowTime)
                    .ThenInclude(st => st.Movie)
                .ToList();


            var detailedReservations = allReservations
                .Where(r => reservations.Any(res => res.ShowTimeId == r.ShowTimeId &&
                                                    res.Seat == r.Seat &&
                                                    res.Sector == r.Sector))
                .ToList();

            var paymentDetails = detailedReservations.Select(r => new PaymentItem
            {
                Description = $"{r.Ticket.Type}, Sector {r.Sector}, Seat {r.Seat}",
                Amount = (decimal)r.Ticket.Price,
            }).ToList();

            return View("PaymentDetails", paymentDetails);
        }

        [HttpPost]
        public IActionResult ProcessPayment(List<Ticket> tickets)
        {
            if (tickets == null || !tickets.Any())
            {
                return RedirectToAction("Index", "Home"); 
            }

            var paymentData = tickets.Select(r => new PaymentItem
            {
                Description = $"Ticket type {r.Type}",
                Amount = (decimal)r.Price
            }).ToList();

            var successUrl = Url.Action("Success", "Payment", null, Request.Scheme);
            var cancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme); 

            if (string.IsNullOrEmpty(successUrl) || string.IsNullOrEmpty(cancelUrl))
            {
                return BadRequest("Invalid success or cancel URL.");
            }

            var paymentUrl = _paymentService.CreatePaymentSession(paymentData, successUrl, cancelUrl);

            return Redirect(paymentUrl);
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View("Success");
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            return View("Cancel");
        }
    }
}
