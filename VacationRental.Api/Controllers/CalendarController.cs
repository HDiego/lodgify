using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public CalendarController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        public ActionResult<CalendarViewModel> Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
            {
                return BadRequest("Nights must be positive");
            }
            if (!_rentals.ContainsKey(rentalId))
            {
                return BadRequest("Rental not found");
            }

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };

            var bookingsRental = _bookings.Values.Where(x => x.RentalId == rentalId).ToList();
            for (var i = 0; i < nights; i++)
            {
                var list = new List<CalendarBookingViewModel>();
                foreach (var booking in bookingsRental)
                {
                    if (booking.Start <= start.Date.AddDays(i) && booking.Start.AddDays(booking.Nights) > start.Date.AddDays(i))
                    {
                        list.Add(new CalendarBookingViewModel
                        {
                            Id = booking.Id,
                            Unit = booking.Unit
                        });
                    }
                }
                if (list.Count > 0)
                {
                    var date = new CalendarDateViewModel
                    {
                        Date = start.Date.AddDays(i),
                        Bookings = list
                    };
                    result.Dates.Add(date);
                }
            }

            var preparationTimeInDays = _rentals[rentalId].PreparationTimeInDays;
            if (preparationTimeInDays > 0)
            {
                var bookingEnds = bookingsRental.Where(x => x.Start.Date <= start.Date && x.Start.AddDays(x.Nights).Date <= start.AddDays(nights).Date).ToList();
                foreach (var booking in bookingEnds)
                {
                    CalendarDateViewModel date = null;
                    for (int i = 0; i < preparationTimeInDays; i++)
                    {
                        if(start.AddDays(nights).Date > booking.Start.AddDays(booking.Nights + i).Date)
                        {
                            var findPreparationDate = result.Dates.Find(x => x.PreparationTimes.Count > 0 && x.Date.Date == booking.Start.AddDays(booking.Nights + i).Date);
                            if (findPreparationDate != null)
                            {
                                findPreparationDate.PreparationTimes.Add(new CalendarPreparationTimeViewModel
                                {
                                    Unit = booking.Unit
                                });
                            }
                            else
                            {
                                date = new CalendarDateViewModel
                                {
                                    Date = booking.Start.AddDays(booking.Nights + i)
                                };
                                date.PreparationTimes.Add(new CalendarPreparationTimeViewModel
                                {
                                    Unit = booking.Unit
                                });
                            }
                            if (date != null)
                            {
                                result.Dates.Add(date);
                            }
                        }
                        
                    }
                }
            }
            
            if (result.Dates.Count != nights)
            {
                for(int i = 0; i< nights; i++)
                {
                    if(!result.Dates.Exists(x => x.Date.Date == start.AddDays(i).Date))
                    {
                        var date = new CalendarDateViewModel
                        {
                            Date = start.AddDays(i).Date
                        };
                        result.Dates.Add(date);
                    }
                }
            }
            return Ok(result);
        }
    }
}
