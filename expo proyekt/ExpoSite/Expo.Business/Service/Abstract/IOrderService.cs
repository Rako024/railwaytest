using Expo.Business.DTOs.OrderDtos;
using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IOrderService
    {

        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task ConfirmOrderAsync(int orderId);
        Task CancelOrderAsync(int orderId);
        Task<Order> CreateOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);
    }
}
