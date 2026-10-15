using Polly;

namespace ApniDukaan.Orders.Business.Policies
{
    public interface IProductsMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
        IAsyncPolicy<HttpResponseMessage> GetBulkHeadIsolationPolicy();
        IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
    }
}
