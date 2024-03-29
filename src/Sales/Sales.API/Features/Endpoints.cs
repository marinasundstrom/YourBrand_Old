using YourBrand.Sales.API.Features.OrderManagement;
using YourBrand.Sales.API.Features.Subscriptions;

namespace YourBrand.Sales.API.Features;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapFeaturesEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapOrderManagementEndpoints()
            .MapSubscriptionEndpoints();

        return app;
    }
}