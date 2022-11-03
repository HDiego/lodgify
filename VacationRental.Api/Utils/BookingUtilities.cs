using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Utils
{
    public static class BookingUtilities
    {
        public static int CalculateUnitsOccuped(List<BookingViewModel> bookingsList, RentalViewModel rental, DateTime startDate, int nights)
        {
            int count = 0;
            foreach (var booking in bookingsList)
            {
                if (booking.RentalId == rental.Id)
                {
                    if ((booking.Start <= startDate.Date && booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) > startDate.Date)
                    || (booking.Start < startDate.AddDays(nights + rental.PreparationTimeInDays) && booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) >= startDate.AddDays(nights + rental.PreparationTimeInDays))
                    || (booking.Start > startDate && booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) < startDate.AddDays(nights + rental.PreparationTimeInDays)))
                    {
                        count++;
                    }
                }   
            }
            return count;
        }

        public static bool UpdatePreparationTime(List<BookingViewModel> bookingsList, int preparationTimeInDays, int unitsAvailable)
        {
            for(int i = 0; i < bookingsList.Count; i++)
            {
                int count = 0;
                var firstBooking = bookingsList[i];
                for(int j = i + 1; j < bookingsList.Count; j++)
                {
                    var secondBooking = bookingsList[j];
                    if((firstBooking.Start <= secondBooking.Start && firstBooking.Start.AddDays(firstBooking.Nights + preparationTimeInDays) > secondBooking.Start)
                        || (firstBooking.Start < secondBooking.Start.AddDays(secondBooking.Nights + preparationTimeInDays) && firstBooking.Start.AddDays(firstBooking.Nights + preparationTimeInDays) >= secondBooking.Start.AddDays(secondBooking.Nights + preparationTimeInDays))
                        || (firstBooking.Start > secondBooking.Start && firstBooking.Start.AddDays(firstBooking.Nights + preparationTimeInDays) < secondBooking.Start.AddDays(secondBooking.Nights + preparationTimeInDays)))
                    {
                        count++;
                    }
                }
                if(count >= unitsAvailable)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
