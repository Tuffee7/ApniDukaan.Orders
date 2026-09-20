using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ApniDukaan.Orders.Data.Entities;
using ApniDukaan.Orders.Data.RepositoryContract;

namespace ApniDukaan.Orders.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly string collectionName = "orders";

        public OrdersRepository(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>(collectionName);
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            var result = await _orders.Find(Builders<Order>.Filter.Empty).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            var result = await _orders.Find(filter).ToListAsync();
            return result;
        }

        public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter)
        {
            return await _orders.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Order?> AddOrder(Order order)
        {
            order.OrderID = Guid.NewGuid();
            order._id = order.OrderID; // Set _id to the same value as OrderID

            foreach (var item in order.OrderItems)
            {
                item._id = Guid.NewGuid();
            }

            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<Order?> UpdateOrder(Order order)
        {
            var filter = Builders<Order>.Filter.Eq(o => o._id, order._id);

            var existingOrder = await _orders.Find(filter).FirstOrDefaultAsync();

            if (existingOrder == null)
                return null;

            order._id = existingOrder._id; // Ensure the _id remains the same

            var result = await _orders.ReplaceOneAsync(filter, order);
            
            if (result.IsAcknowledged && result.ModifiedCount > 0)
                return order;

            return null;
        }

        public async Task<bool> DeleteOrder(Guid orderID)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderID, orderID);

            var existingOrder = await _orders.Find(filter).FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;

            var result = await _orders.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
