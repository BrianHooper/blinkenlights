﻿using LiteDbLibrary;
using LiteDbLibrary.Schemas;

namespace LiteDbOperator
{
	public class Program
	{
		private LiteDbHandler dbHandler { get; set; }

		public List<PackageTrackingItem> Packages()
		{
			return new List<PackageTrackingItem>()
			{
				new PackageTrackingItem()
				{
					Name = "Powder Coat",
					TrackingNumber = "1Z254528YW91175832",
					Carrier = "UPS",
					Url = "https://www.ups.com/track?track=yes&trackNums=1Z254528YW91175832&loc=en_US&requester=ST/trackdetails"
				},
				new PackageTrackingItem()
				{
					Name = "Multimeter",
					TrackingNumber = "9405508205499681686715",
					Carrier = "USPS",
					Url = "https://tools.usps.com/go/TrackConfirmAction_input?strOrigTrackNum=9405508205499681686715"
				},
			};
		}

		public List<ModuleItem> Modules()
		{
			return new List<ModuleItem>()
			{

                // Row 1
                new ModuleItem(name: "Time", endpoint: "/Modules/GetTimeModule", refreshRateMs: 60 * 1000, row: 1, col: 1),
				new ModuleItem(name: "Weather", endpoint: "/Modules/GetWeatherData", refreshRateMs: 15 * 60 * 1000, row: 1, col: 2, colSpan: 5),
				new ModuleItem(name: "IssTracker", endpoint: "/Modules/GetIssTrackerModule", refreshRateMs: 5 * 60 * 1000, row: 1, col: 7, colSpan: 2),

                // Row 2
                new ModuleItem(name: "WWII", endpoint: "/Modules/GetWWIIModule", refreshRateMs: 3 * 60 * 60 * 1000, row: 2, col: 1, colSpan: 2),
				new ModuleItem(name: "Headlines", endpoint: "/Modules/GetHeadlinesModule", refreshRateMs: 60 * 60 * 1000, row: 2, col: 3, colSpan: 4),
				new ModuleItem(name: "Stock", endpoint: "/Modules/GetStockModule", refreshRateMs: 3 * 60 * 60 * 1000, row: 2, col: 7, colSpan: 2),

                // Row 3
                new ModuleItem(name: "Calendar", endpoint: "/Modules/GetCalendarModule", refreshRateMs: 15 * 60 * 1000, row: 3, col: 1, colSpan: 2),
				new ModuleItem(name: "Utility", endpoint: "/Modules/GetUtilityData", refreshRateMs: 15 * 60 * 1000, row: 3, col: 3, colSpan: 2),
				new ModuleItem(name: "Life360", endpoint: "/Modules/GetLife360Module", refreshRateMs: 2 * 60 * 1000, row: 3, col: 5, colSpan: 2),
				new ModuleItem(name: "Slideshow", endpoint: "/Modules/GetSlideshowModule", refreshRateMs: 3 * 60 * 60 * 1000, row: 3, col: 7, colSpan: 2),
			};
		}

		public List<CountdownItem> CountdownTimers()
		{
			return new List<CountdownItem>()
			{
				new CountdownItem()
				{
					Date = new DateTime(2024, 5, 16),
					Name = "Naynay's party"
				},
				new CountdownItem()
				{
					Date = new DateTime(2024, 8, 10),
					Name = "Camran Wedding"
				},
				new CountdownItem()
				{
					Date = new DateTime(2024, 8, 27),
					Name = "Burning Man"
				},
				new CountdownItem()
				{
					Date = new DateTime(2024, 12, 24),
					Name = "Christmas"
				}
			};
		}

		public void Run()
		{
			dbHandler.Clear();
			dbHandler.Upsert(CountdownTimers());
			dbHandler.Upsert(Modules());
			dbHandler.Upsert(Packages());

			var report = dbHandler.ReadFull();
			report.ToList().ForEach(r =>
			{
				r.Value.ForEach(i =>
				{
					Console.WriteLine($"{r.Key}\t{i.ToString()}");
				});
			});
		}

		public Program()
		{
			var relativeDbPath = Path.Combine(@"..\..\..\..\blinkenlights\Blinkenlights\wwwroot", LiteDbHandler.DATABASE_FILENAME);
			this.dbHandler = new LiteDbHandler(Path.GetFullPath(relativeDbPath));
		}

		static void Main(string[] args)
		{
			new Program().Run();
		}
	}
}