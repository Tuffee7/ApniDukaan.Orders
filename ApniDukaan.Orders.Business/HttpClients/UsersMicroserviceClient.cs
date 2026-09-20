using ApniDukaan.Orders.Business.ResponseDTO;
using System.Net;
using System.Net.Http.Json;

namespace ApniDukaan.Orders.Business.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;

        public UsersMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDTO?> GetUserByID(Guid userID)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Users/{userID}");

            if (response.IsSuccessStatusCode)
            {
                UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

                if (user == null)
                {
                    throw new Exception($"User with ID {userID} not found in the response.");
                }

                return user;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException($"Bad request when fetching user with ID {userID}.", null, response.StatusCode);
                }
                else
                {
                    throw new HttpRequestException($"Error fetching user with ID {userID}. Status code: {response.StatusCode}", null, response.StatusCode);
                }
            }

        }
    }
}
