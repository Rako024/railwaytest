
using Expo.Business.DTOs.UserDtos;
using Expo.Business.Exceptions;
using Expo.Business.Profiles;
using Expo.Business.Profilies;
using Expo.Business.Service;
using Expo.Core.Entities;
using Expo.Core.HelperEntities;
using Expo.Data.DAL;
using Expo.Data.Repositories;
using Expo.Data.Repositories.Abstracts;
using Expo.Data.Repositories.Concretes;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ExpoSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            

            builder.Services.AddIdentity<AppUser,IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ExpoContext>().AddDefaultTokenProviders();

            builder.Services.AddDbContext<ExpoContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Server"));
            });

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"])),
                    LifetimeValidator = (_, expireDate, token, _) => token != null ? expireDate > DateTime.UtcNow : false
                };
            });

            builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddServicesRepository(builder.Configuration);
            
            builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddAutoMapper(typeof(UserMapProfilies));
            builder.Services.AddAutoMapper(typeof(CategoryMapProfilies));
            builder.Services.AddAutoMapper(typeof(ProductMapProfilies));
            builder.Services.AddAutoMapper(typeof(OrderMapProfiles));
            builder.Services.AddAutoMapper(typeof(WishlistMapProfiles));
            builder.Services.AddHttpContextAccessor();



        //    builder.Services.AddDataProtection()
        //.PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"))
        //.ProtectKeysWithDpapi();


            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });


                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                    });
            });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
                options.AddPolicy("AdminOrCustomerPolicy", policy => policy.RequireRole("Admin", "Customer")); 
            });


            //builder.Services.AddControllers()
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //});

            builder.Services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserRegisterDtoValidation>())
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                        .ToArray();

                    var errorResponse = new
                    {
                        StatusCode = 400,
                        Message = errors
                    };

                    return new JsonResult(errorResponse);
                };
            });


            var app = builder.Build();

            app.UseStaticFiles();
            // Configure the HTTP request pipeline.

            app.UseSwagger();
                app.UseSwaggerUI();

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseCors("corsapp");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
