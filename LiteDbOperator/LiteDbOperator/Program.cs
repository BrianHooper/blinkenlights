using LiteDbLibrary;
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
				//new PackageTrackingItem()
				//{
				//	Name = "Item",
				//	TrackingNumber = "1ZY549V70290264835",
				//	Provider = "USPS",
				//	Url = ""
				//},
				//new PackageTrackingItem()
				//{
				//	Name = "Item",
				//	TrackingNumber = "1ZY549V70290264836",
				//	Provider = "UPS",
				//	Url = ""
				//},
				//new PackageTrackingItem()
				//{
				//	Name = "Item",
				//	TrackingNumber = "638012418280",
				//	Provider = "Fedex",
				//	Url = ""
				//},
				//new PackageTrackingItem()
				//{
				//	Name = "Item",
				//	TrackingNumber = "1Z023E2X0214323462",
				//	Provider = "Amazon",
				//	Url = ""
				//},
			};
		}

		public List<ModuleItem> Modules()
		{
			return new List<ModuleItem>()
			{

                // Row 1
                new ModuleItem(name: "TimeRoot", row: 1, col: 1),
				new ModuleItem(name: "WeatherRoot", row: 1, col: 2, colSpan: 5),
				new ModuleItem(name: "IssTrackerRoot", row: 1, col: 7, colSpan: 2),
				//new ModuleItem(name: "Life360Root", row: 1, col: 4),

                // Row 2
                new ModuleItem(name: "WWIIRoot", row: 2, col: 1, colSpan: 2),
				new ModuleItem(name: "HeadlinesRoot", row: 2, col: 3, colSpan: 4),
				new ModuleItem(name: "FinanceAnswerRoot", row: 2, col: 7, colSpan: 2),

                // Row 3
                new ModuleItem(name: "CalendarRoot", row: 3, col: 1, colSpan: 2),
				new ModuleItem(name: "UtilityRoot", row: 3, col: 3, colSpan: 2),
				new ModuleItem(name: "StatusRoot", row: 3, col: 5, colSpan: 2),
				new ModuleItem(name: "SlideshowRoot", row: 3, col: 7, colSpan: 2),
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