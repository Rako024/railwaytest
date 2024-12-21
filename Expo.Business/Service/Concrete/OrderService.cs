using AutoMapper;
using Expo.Business.DTOs.OrderDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        // Sifarişlərin hamısını əldə et
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync(
                include: query => query
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(o => o.AppUser)
            );

            if (orders == null || !orders.Any())
            {
                throw new GlobalAppException("No orders found.");
            }

            return _mapper.Map<List<OrderDto>>(orders);
        }



        // ID ilə konkret sifarişi əldə et
        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetAsync(
                o => o.Id == orderId,
                include: query => query
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(o => o.AppUser)
            );

            if (order == null)
            {
                throw new GlobalAppException("Order not found.");
            }

            return _mapper.Map<OrderDto>(order);
        }

        // Admin tərəfindən sifarişi təsdiqlə
        public async Task ConfirmOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetAsync(o => o.Id == orderId);
            if (order == null)
                throw new GlobalAppException("Order not found!");

            if (order.Status == "Confirmed")
                throw new GlobalAppException("Order is already confirmed!");

            order.Status = "Confirmed";
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.Save();
        }

        // Admin tərəfindən sifarişi ləğv et
        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetAsync(o => o.Id == orderId);
            if (order == null)
                throw new GlobalAppException("Order not found!");

            if (order.Status == "Cancelled")
                throw new GlobalAppException("Order is already cancelled!");

            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.Save();
        }

        // Yeni sifariş yarat
        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new GlobalAppException("Order cannot be null!");

            await _orderRepository.AddAsync(order);
            await _orderRepository.Save();
            return order;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found!");

            await _orderRepository.HardDeleteAsync(order);
            await _orderRepository.Save();
        }

    }
}
