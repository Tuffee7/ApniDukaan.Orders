using ApniDukaan.Orders.Business.ResponseDTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApniDukaan.Orders.Business.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;

        public ProductsMicroserviceClient(HttpClient httpClient, ILogger<ProductsMicroserviceClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<ProductDTO?> GetProductByProductID(Guid productID)
        {
            try
            {
                string cacheKeyToRead = $"product:{productID}";
                string? cachedProduct = await _distributedCache.GetStringAsync(cacheKeyToRead);
                
                //if (cachedProduct != null)
                //{
                //    ProductDTO? productDTO = JsonSerializer.Deserialize<ProductDTO>(cachedProduct);

                //    _logger.LogInformation($"Product with ID {productID} retrieved from cache.");

                //    return productDTO;
                //}

                HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/product-id/{productID}");

                if (!response.IsSuccessStatusCode)
                {
                    // Don't add data to cache when FallbackPolicy is triggered and the response status code is 503 (Service Unavailable)
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        ProductDTO? productFromFallbackPolicy = await response.Content.ReadFromJsonAsync<ProductDTO>();

                        if (productFromFallbackPolicy == null)
                        {
                            throw new HttpRequestException("Fallback data not available", null, HttpStatusCode.ServiceUnavailable);
                        }

                        return productFromFallbackPolicy;
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException("Bad request", null, HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                    }
                }


                ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();

                if (product == null)
                {
                    throw new ArgumentException("Invalid Product ID");
                }

                // Key: product:{productID}
                // Value: { "ProductID": "guid", "ProductName": "name", "Category": "category", "Price": 0, "QuantityInStock": 0 }
                string cacheKey = $"product:{productID}";
                string productJson = JsonSerializer.Serialize(product);
                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(240), // Cache for 120 seconds
                    SlidingExpiration = TimeSpan.FromSeconds(120) // Reset expiration if accessed within 120 seconds
                };
                _distributedCache.SetStringAsync(cacheKey, productJson, cacheOptions);

                return product;
            }
            catch (BulkheadRejectedException ex)
            {
                _logger.LogError(ex, "Bulkhead isolation blocks the request since the request queue is full");

                return new ProductDTO
                {
                    ProductID = Guid.Empty,
                    ProductName = "Temporarily Unavailable (Bulkhead)",
                    Category = "Temporarily Unavailable (Bulkhead)",
                    Price = 0,
                    QuantityInStock = 0
                };
            }
        }
    }
}
