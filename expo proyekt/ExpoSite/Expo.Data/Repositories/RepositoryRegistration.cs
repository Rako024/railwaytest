using Expo.Data.Repositories.Abstracts;
using Expo.Data.Repositories.Concretes;
using MailKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Data.Repositories
{
    public static class RepositoryRegistration
    {
        public static void AddServicesRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketItemRepository, BasketItemRepository>();
            services.AddScoped<IOrderRepository,OrderRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
        }
    }
}
