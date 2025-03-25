using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.ZaloPay;
using CafeteriaOrdering.API.ZaloPay.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

using CafeteriaOrdering.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CafeteriaOrdering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not found in .env file.");
            }

            builder.Services.AddDbContext<CafeteriaOrderingDBContext>(options =>
                options.UseSqlServer(connectionString)
            );

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("MANAGER", policy => policy.RequireClaim("Role", "MANAGER"));
                options.AddPolicy("DELIVER  ", policy => policy.RequireClaim("Role", "DELIVER"));
                options.AddPolicy("PATRON", policy => policy.RequireClaim("Role", "PATRON"));
            });

            var zaloPayConfig = builder.Configuration.GetSection("ZaloPay");
            var appId = zaloPayConfig["AppId"];
            var key1 = zaloPayConfig["Key1"];
            var key2 = zaloPayConfig["Key2"];
            var endpoint = zaloPayConfig["Endpoint"];

            builder.Services.Configure<ZaloPayConfig>(zaloPayConfig);
            builder.Services.AddHttpClient<ZaloPayService>();
            builder.Services.AddScoped<IMealDeliveryService, MealDeliveryService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<LoggingService>();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Tắt "$id"
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());


            // 🔥 Sử dụng scope để lấy Scoped Services
            using (var scope = app.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var orderService = scopedServices.GetRequiredService<IMealDeliveryService>();
                var notificationService = scopedServices.GetRequiredService<NotificationService>();
                var loggingService = scopedServices.GetRequiredService<LoggingService>();

                // Subscribe listeners to the event
                orderService.OnOrderStatusChanged += notificationService.SendNotification;
                orderService.OnOrderStatusChanged += loggingService.LogStatusChange;
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
