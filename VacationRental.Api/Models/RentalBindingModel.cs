﻿using System.ComponentModel.DataAnnotations;

namespace VacationRental.Api.Models
{
    public class RentalBindingModel
    {
        [Required]
        public int Units { get; set; }
        [Required]
        public int PreparationTimeInDays { get; set; }
    }
}
