using ApniDukaan.Orders.Business.Mappers;
using ApniDukaan.Orders.Business.ServiceContract;
using ApniDukaan.Orders.Business.Services;
using ApniDukaan.Orders.Business.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApniDukaan.Orders.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: Add your business layer services into IoC Container

            services.AddValidatorsFromAssemblyContaining<OrderItemAddRequestValidator>();
            services.AddAutoMapper(typeof(OrderAddRequestToOrderMappingProfile).Assembly);
            services.AddScoped<IOrdersService, OrdersService>();

            return services;
        }
    }
}
