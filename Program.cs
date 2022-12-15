using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using System.Configuration;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.Extensions.Options;
using SalesWebMvc.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

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

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<SellerService>();
            builder.Services.AddScoped<DepartmentService>();
            builder.Services.AddScoped<SalesRecordService>();

            var app = builder.Build();
            
            // Seeding the database
            SeedDatabase();

            // Configure the HTTP request pipeline.
            var enUS = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(enUS),
                SupportedCultures = new List<CultureInfo> { enUS },
                SupportedUICultures = new List<CultureInfo> { enUS }
            };

            app.UseRequestLocalization(localizationOptions);
            
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Seeding method
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