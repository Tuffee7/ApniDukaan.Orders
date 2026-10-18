using ApniDukaan.Orders.Business.ResponseDTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApniDukaan.Orders.Business.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;
        public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<UserDTO?> GetUserByUserID(Guid userID)
        {
            try
            {
                string cacheKeyToRead = $"user:{userID}";
                string? cachedUser = await _distributedCache.GetStringAsync(cacheKeyToRead);

                //if (cachedUser != null)
                //{
                //    UserDTO? userDTO = JsonSerializer.Deserialize<UserDTO>(cachedUser);

                //    _logger.LogInformation($"User with ID {userID} retrieved from cache.");

                //    return userDTO;
                //}

                HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/users/{userID}");

                if (!response.IsSuccessStatusCode)
                {
                    //if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    //{
                    //    UserDTO? userFromFallbackPolicy = await response.Content.ReadFromJsonAsync<UserDTO>();

                    //    if (userFromFallbackPolicy == null)
                    //    {
                    //        throw new NotImplementedException();
                    //    }

                    //    return userFromFallbackPolicy;
                    //}

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

                UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

                if (user == null)
                {
                    throw new ArgumentException("Invalid User ID");
                }

                // Key: user:{userID}
                // Value: { "UserID": "guid", "Email": "email", "PersonName": "name", "Gender": "gender" }
                string cacheKey = $"user:{userID}";
                string userJson = JsonSerializer.Serialize(user);
                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(240), // Cache for 120 seconds
                    SlidingExpiration = TimeSpan.FromSeconds(120) // Reset expiration if accessed within 120 seconds
                };
                _distributedCache.SetStringAsync(cacheKey, userJson, cacheOptions);

                return user;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Request failed because Circuit Breaker is open. Users microservice is temporarily unavailable. Returning Dummy Data");

                return new UserDTO
                {
                    UserID = Guid.Empty,
                    Email = "Temporarily Unavailable (Circuit Breaker)",
                    PersonName = "Temporarily Unavailable (Circuit Breaker)",
                    Gender = "Temporarily Unavailable (Circuit Breaker)"
                };
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "Request failed due to timeout. Users microservice is temporarily unavailable. Returning Dummy Data");

                return new UserDTO
                {
                    UserID = Guid.Empty,
                    Email = "Temporarily Unavailable (timeout)",
                    PersonName = "Temporarily Unavailable (timeout)",
                    Gender = "Temporarily Unavailable (timeout)"
                };
            }
        }
    }
}
