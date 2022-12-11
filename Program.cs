using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using System.Configuration;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.Extensions.Options;

namespace SalesWebMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = "server=localhost;user=developer;password=0000;database=saleswebmvcappdb";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

            builder.Services.AddDbContext<SalesWebMvcContext>(dbContextOptions => dbContextOptions.UseMySql(connectionString, serverVersion));

            // builder.Services.AddScoped<SeedingService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            SeedDatabase();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            void SeedDatabase()
            {
                using var scope = app.Services.CreateScope();
                try
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<SalesWebMvcContext>();
                    SeedingService.Seed(scopedContext);
                }
                catch
                {
                    throw;
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}