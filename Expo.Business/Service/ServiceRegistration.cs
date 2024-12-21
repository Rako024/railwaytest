using Expo.Business.Service.Abstract;
using Expo.Business.Service.Concrete;
using Expo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketItemService, BasketItemService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IWishlistService, WishlistService>();
        }
    }
}
