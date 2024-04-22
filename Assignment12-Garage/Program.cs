using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Assignment12_Garage.Data;
namespace Assignment12_Garage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<Assignment12_GarageContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Assignment12_GarageContext") ?? throw new InvalidOperationException("Connection string 'Assignment12_GarageContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");
                //pattern: "{controller=Vehicles}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
