using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.Services;

namespace CafeteriaOrdering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<CafeteriaOrderingDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IMealDeliveryService, MealDeliveryService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<LoggingService>();

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
