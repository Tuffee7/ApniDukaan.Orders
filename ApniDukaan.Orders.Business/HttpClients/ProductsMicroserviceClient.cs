using ApniDukaan.Orders.Business.ResponseDTO;
using System.Net;
using System.Net.Http.Json;

namespace ApniDukaan.Orders.Business.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;

        public ProductsMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDTO?> GetProductByProductID(Guid productID)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/product-id/{productID}");

            if (response.IsSuccessStatusCode)
            {
                ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();

                if (product == null)
                {
                    throw new Exception($"Product with ID {productID} not found in the response.");
                }

                return product;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException($"Bad request when fetching product with ID {productID}.", null, response.StatusCode);
                }
                else
                {
                    throw new HttpRequestException($"Error fetching product with ID {productID}. Status code: {response.StatusCode}", null, response.StatusCode);
                }
            }

        }
    }
}
