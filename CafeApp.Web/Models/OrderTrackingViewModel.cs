using CafeApp.Core.Entities;

namespace CafeApp.Web.Models
{
    public class OrderTrackingViewModel
    {
        public Order? Order { get; set; }
        public string? OrderNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
