using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ApniDukaan.Orders.Business.Policies
{
    public class UsersMicroservicePolicies : IUsersMicroservicePolicies
    {
        public readonly ILogger<UsersMicroservicePolicies> _logger;
        public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
                    });

            return policy;
        }
    }
}
