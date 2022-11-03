using System.ComponentModel.DataAnnotations;

namespace VacationRental.Api.Models
{
    public class RentalUpdateModel
    {
        [Required]
        public int RentalId { get; set; }
        [Required]
        public int Units { get; set; }
        [Required]
        public int PreparationTimeInDays { get; set; }
    }
}
