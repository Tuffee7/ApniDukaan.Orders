using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ApniDukaan.Orders.Data.Entities;

namespace ApniDukaan.Orders.Data.RepositoryContract
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Order>> GetOrders();

        Task<IEnumerable<Order>> GetOrdersByCondition(FilterDefinition<Order> filter);

        Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter);

        Task<Order?> AddOrder(Order order);

        Task<Order?> UpdateOrder(Order order);

        Task<bool> DeleteOrder(Guid orderID);
    }
}
