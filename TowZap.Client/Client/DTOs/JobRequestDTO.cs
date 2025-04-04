namespace TowZap.Client.Client.DTOs
{
    public class JobRequestDTO
    {
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public string Problem { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string Notes { get; set; }
        public decimal TowAmount { get; set; }
    }
}
