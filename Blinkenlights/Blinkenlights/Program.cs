using Blinkenlights.Data;
using Blinkenlights.Data.LiteDb;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Transformers;
using LiteDbLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blinkenlights
{
    public static class BuilderServicesExtensions
    {
        public static T Get<T>(this IServiceCollection services) where T : class
        {
			return services.FirstOrDefault(s => s.ServiceType == typeof(T))?.ImplementationInstance as T;
		}
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

			builder.Services.AddSingleton<IApiHandler, ApiHandler>();
            builder.Services.AddSingleton<ILiteDbHandler, LiteDbHandler>(x =>
            {
				return LiteDbFactory.Build(builder.Services.Get<IWebHostEnvironment>());
			});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Root/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Root}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}