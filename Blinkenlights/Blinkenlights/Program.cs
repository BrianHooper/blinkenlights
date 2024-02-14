using Blinkenlights.Data;
using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blinkenlights.DatabaseHandler
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
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IDatabaseHandler, DatabaseHandler>();
            builder.Services.AddSingleton<IApiHandler, ApiHandler>();

            builder.Services.AddSingleton<IDataFetcher<CalendarModuleData>, CalendarDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<HeadlinesData>, HeadlinesDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<IndexModuleData>, IndexDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<IssTrackerData>, IssDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<Life360Data>, Life360DataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<SlideshowData>, SlideshowDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<StockData>, StockDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<TimeData>, TimeDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<UtilityData>, UtilityDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<WeatherData>, WeatherDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<WWIIData>, WWIIDataFetcher>();

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

            app.MapGet("/read/{key}", (string key, IDatabaseHandler db) =>
            {
                if (db.TryRead(key, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    return Results.Content(value);
                }

                return Results.NoContent();
            });

            app.MapPut("/write/{key}", (string key, string value, IDatabaseHandler db) =>
            {
                if (db.Write(key, value))
                {
                    return Results.Ok();
                }
                else
                {
                    return Results.Problem();
                }
            });

            app.Run();
        }
    }
}