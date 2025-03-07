using CafeteriaOrdering.API.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<CafeteriaOrderingDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

            var app = builder.Build();

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
