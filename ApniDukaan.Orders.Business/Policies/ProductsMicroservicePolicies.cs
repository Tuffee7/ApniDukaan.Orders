using ApniDukaan.Orders.Business.ResponseDTO;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.Fallback;
using Polly.Wrap;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ApniDukaan.Orders.Business.Policies
{
    public class ProductsMicroservicePolicies : IProductsMicroservicePolicies
    {
        private readonly ILogger<ProductsMicroservicePolicies> _logger;
        public ProductsMicroservicePolicies(ILogger<ProductsMicroservicePolicies> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            AsyncFallbackPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .FallbackAsync(async (context) =>
                {
                    _logger.LogWarning("Fallback triggered for Products Microservice. Returning dummy data.");

                    ProductDTO product = new ProductDTO
                    {
                        ProductID = Guid.Empty,
                        ProductName = "Dummy Product (Fallback)",
                        Category = "Dummy Category (Fallback)",
                        Price = 0.0,
                        QuantityInStock = 0
                    };

                    var response =  new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json")
                    };

                    return response;
                });
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetBulkHeadIsolationPolicy()
        {
            AsyncBulkheadPolicy<HttpResponseMessage> policy = Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 2, // Maximum number of concurrent executions
                maxQueuingActions: 40, // Maximum number of actions that can be queued
                onBulkheadRejectedAsync: (context) =>
                {
                    _logger.LogWarning("Bulkhead isolation triggered for Products Microservice.Cannot send any more requests as queue is full.");
                    throw new BulkheadRejectedException("Bulkhead isolation triggered for Products Microservice. Cannot send any more requests as queue is full.");
                }
                );
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
        {
            var fallbackPolicy = GetFallbackPolicy();
            var bulkheadPolicy = GetBulkHeadIsolationPolicy();

            var combinedPolicy = Policy.WrapAsync(fallbackPolicy, bulkheadPolicy);

            return combinedPolicy;
        }
    }
}
