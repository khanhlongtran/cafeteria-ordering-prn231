using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.ZaloPay;
using CafeteriaOrdering.API.ZaloPay.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

using CafeteriaOrdering.API.Services;

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

            var zaloPayConfig = builder.Configuration.GetSection("ZaloPay");
            var appId = zaloPayConfig["AppId"];
            var key1 = zaloPayConfig["Key1"];
            var key2 = zaloPayConfig["Key2"];
            var endpoint = zaloPayConfig["Endpoint"];

            builder.Services.Configure<ZaloPayConfig>(zaloPayConfig);
            builder.Services.AddHttpClient<ZaloPayService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Get service instances
            var orderService = app.Services.GetRequiredService<IMealDeliveryService>();
            var notificationService = app.Services.GetRequiredService<NotificationService>();
            var loggingService = app.Services.GetRequiredService<LoggingService>();

            // Subscribe listeners to the event
            orderService.OnOrderStatusChanged += notificationService.SendNotification;
            orderService.OnOrderStatusChanged += loggingService.LogStatusChange;

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
