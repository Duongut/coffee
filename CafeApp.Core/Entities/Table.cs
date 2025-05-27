using System.ComponentModel.DataAnnotations;

namespace CafeApp.Core.Entities
{
    public class Table
    {
        public int Id { get; set; }
        
        [Required]
        public int TableNumber { get; set; }
        
        public bool IsOccupied { get; set; }
        
        public string? SessionId { get; set; }
        
        public DateTime? OccupiedAt { get; set; }
        
        public DateTime? LastActivityAt { get; set; }
        
        public int Capacity { get; set; } = 4;
        
        public bool IsActive { get; set; } = true;
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
