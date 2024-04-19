namespace Assignment12_Garage.Models.ViewModels
{
    public class VehicleViewModel
    {
        public int Id { get; set; }
        public VehicleType VehicleType { get; set; }
        public string RegNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
    
        public string? ParkingSpot { get; set; }
    }
}
