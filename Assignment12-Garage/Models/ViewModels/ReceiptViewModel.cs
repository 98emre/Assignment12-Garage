namespace Assignment12_Garage.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public int Id { get; set; }
        public string RegNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime CheckoutDate { get; set; }
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
