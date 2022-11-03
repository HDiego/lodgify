using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Utils;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public ActionResult<BookingViewModel> Get(int bookingId)
        {
            if (!_bookings.ContainsKey(bookingId))
            {
                return BadRequest("Booking not found");
            }    

            return Ok(_bookings[bookingId]);
        }

        [HttpPost]
        public ActionResult<ResourceIdViewModel> Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
            {
                return BadRequest("Nigts must be positive");
            }
            if (!_rentals.ContainsKey(model.RentalId))
            {
                return BadRequest("Rental not found");
            }

            int count = 0;
            for (var i = 0; i < model.Nights; i++)
            {
                var rental = _rentals[model.RentalId];
                count = BookingUtilities.CalculateUnitsOccuped(_bookings.Values.ToList(), rental, model.Start, model.Nights);
                
                if (count >= _rentals[model.RentalId].Units)
                {
                    return BadRequest("Not available");
                }
            }

            if(count >= 0)
            {
                count++;
            }
            var key = new ResourceIdViewModel { Id = _bookings.Keys.Count + 1 };

            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                Unit = count
            });

            return Ok(key);
        }
    }
}
