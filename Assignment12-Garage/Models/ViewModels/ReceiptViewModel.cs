using System.ComponentModel.DataAnnotations;

namespace Assignment12_Garage.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Vehicle Type")]
        public VehicleType VehicleType { get; set; }

        [Display(Name = "Registration Number")]
        public string RegNumber { get; set; }

        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }

        [Display(Name = "Checkout Date")]
        public DateTime CheckoutDate { get; set; }

        [Display(Name = "Total Hours of Parking")]
        public int TotalParkingHours { get; set; }
        public double Price { get; set; }

        public void CalculateTotalParkingHours()
        {
            TimeSpan parkingDuration = CheckoutDate - ArrivalDate;
            TotalParkingHours = (int)parkingDuration.TotalHours;
        }

        public void CalculatePrice()
        {
            const double HourlyRate = 50;
            Price = TotalParkingHours * HourlyRate;
        }

    }
}
