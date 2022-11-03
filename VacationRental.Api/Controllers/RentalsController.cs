using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Utils;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public RentalsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public ActionResult<RentalViewModel> Get(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
            {
                return BadRequest("Rental not found");
            }
            return Ok(_rentals[rentalId]);
        }

        [HttpPost]
        public ActionResult<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            });

            return Ok(key);
        }

        [HttpPut]
        public ActionResult<bool> Put(RentalUpdateModel model)
        {
            if(!_rentals.ContainsKey(model.RentalId))
            {
                return BadRequest("Rental not found");
            }
            var rental = _rentals[model.RentalId];
            if(_bookings != null)
            {
                var canUpdate = BookingUtilities.UpdatePreparationTime(_bookings.Values.Where(x => x.RentalId == model.RentalId).ToList(), model.PreparationTimeInDays, model.Units);
                if (!canUpdate)
                {
                    return BadRequest("Can´t update Rental");
                }
            }
            
            rental.Units = model.Units;
            rental.PreparationTimeInDays = model.PreparationTimeInDays;
            return Ok(true);
        }
    }
}
