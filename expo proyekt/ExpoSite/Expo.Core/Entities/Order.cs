using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class Order : BaseEntitiy
    {
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Sifariş tarixi
        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
        public bool IsPaid { get; set; } = false; // Sifarişin ödənməsi
        public double TotalPrice { get; set; } = 0; // Toplam məbləğ
        public string Status { get; set; } = "Pending"; //"Pending", "Confirmed", "Cancelled"
    }
}

