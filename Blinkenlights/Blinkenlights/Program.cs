using Blinkenlights.ApiHandlers;
using Blinkenlights.Data;
using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Net;

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



			builder.Host.UseSerilog();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IDatabaseHandler, DatabaseHandler>();
			builder.Services.AddSingleton<IApiHandler, ApiHandler>();
			builder.Services.AddSingleton<IApiStatusFactory, ApiStatusFactory>();

			builder.Services.AddSingleton<IDataFetcher<CalendarModuleData>, CalendarDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<HeadlinesData>, HeadlinesDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<IndexModuleData>, IndexDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<OuterSpaceData>, OuterSpaceDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<Life360Data>, Life360DataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<SlideshowData>, SlideshowDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<StockData>, StockDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<TimeData>, TimeDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<UtilityData>, UtilityDataFetcher>();
            builder.Services.AddSingleton<IDataFetcher<WeatherData>, WeatherDataFetcher>();
			builder.Services.AddSingleton<IDataFetcher<WWIIData>, WWIIDataFetcher>();
			builder.Services.AddSingleton<IDataFetcher<FlightStatusData>, FlightStatusDataFetcher>();

			var app = builder.Build();

            var webHostEnv = app.Services.GetService<IWebHostEnvironment>();

            var logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            var loggerBootstrap = app.Environment.IsDevelopment() ? new LoggerConfiguration().MinimumLevel.Debug() : new LoggerConfiguration().MinimumLevel.Information();
            loggerBootstrap
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
				.Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: logOutputTemplate)
                .WriteTo.File(
                    System.IO.Path.Combine(webHostEnv.ContentRootPath, "LogFiles", "diagnostics.txt"),
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    retainedFileCountLimit: 2,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    outputTemplate: logOutputTemplate);
            Log.Logger = loggerBootstrap.CreateLogger();

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

            //app.UseHttpsRedirection();
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