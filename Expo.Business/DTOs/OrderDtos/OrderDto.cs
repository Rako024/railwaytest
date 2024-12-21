using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.OrderDtos
{
    public record OrderDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string AppUserFullName { get; set; }
        public string AppUserEmail { get; set; }
        public string AppUserPhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public bool IsPaid { get; set; }
        public double TotalPrice { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
