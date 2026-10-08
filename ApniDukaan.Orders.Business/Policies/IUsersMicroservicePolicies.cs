using Polly;

namespace ApniDukaan.Orders.Business.Policies
{
    public interface IUsersMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
    }
}
