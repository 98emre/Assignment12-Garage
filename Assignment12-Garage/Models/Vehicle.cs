using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment12_Garage.Models
{
    public class Vehicle
    {

        [Key]
        public int Id {  get; set; }

        [Display(Name = "Vehicle Type")]
        [Required(ErrorMessage = "Choose vehicle type")]
        public VehicleType VehicleType { get; set; }

        [Required(ErrorMessage = "Write down the vehicle registration number")]
        [StringLength(10, MinimumLength = 6)]

        [Display(Name = "Registration Number")]
        public string RegNumber { get; set; }

        [Required(ErrorMessage = "Add color for your vehicle")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Add brand for your vehicle")]
        public string Brand { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Add model for your vehicle")]
        public string VehicleModel { get; set; }

        [Display(Name = "Number of Wheels")]
        [Range(0, 100)]
        [Required(ErrorMessage = "Add nr of wheels for your vehicle")]
        public int NrOfWheels {  get; set; }

        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }

        [Column("ParkingSpot")]
        public string? ParkingSpot { get; set; }
    }
}
