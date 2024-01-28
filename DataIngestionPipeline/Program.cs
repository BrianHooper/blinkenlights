using LiteDbLibrary;
using LiteDbLibrary.Schemas;
using System.Reflection;

namespace DataIngestionPipeline
{
	internal class Program
	{
		private LiteDbHandler LiteDb { get; set; }

		private void Run()
		{
			var relativeDbPath = Path.Combine(@"..\..\..\Blinkenlights\Blinkenlights\wwwroot", LiteDbHandler.DATABASE_FILENAME);
			this.LiteDb = new LiteDbHandler(Path.GetFullPath(relativeDbPath));
			var requests = this.LiteDb.Read<PackageTrackingItem>();
			foreach (var req in requests)
			{
				Console.WriteLine(req.Name);
			}
		}

		static void Main(string[] args)
		{
			new Program().Run();
		}
	}
}