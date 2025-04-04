namespace TowZap.Client.Client.DTOs
{
    public class TowingRequestDTO
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string Status { get; set; }
        public string AssignedDriverId { get; set; }
    }
}
