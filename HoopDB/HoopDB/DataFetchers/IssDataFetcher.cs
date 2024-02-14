using Blinkenlights.Dataschemas;

namespace HoopDB
{
	public interface IIssDataFetcher : IDataFetcher
	{
		public IssTrackerData GetData();
	}

	public class IssDataFetcher : DataFetcherBase, IIssDataFetcher
	{
		IDatabaseHandler DatabaseHandler;

		IApiHandler ApiHandler;

		IWebHostEnvironment WebHostEnvironment;

		public IssDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, IWebHostEnvironment webHostEnvironment) : base(TimeSpan.FromMinutes(5))
		{
			this.DatabaseHandler = databaseHandler;
			this.ApiHandler = apiHandler;
			this.WebHostEnvironment = webHostEnvironment;
		}

		public override void FetchData()
		{
			Console.WriteLine("Calling ISS api");
			var issTrackerData = FetchIssTrackerData();
			this.DatabaseHandler.Set(issTrackerData);
		}

		public IssTrackerData GetData()
		{
			return this.DatabaseHandler.Get<IssTrackerData>();
		}

		private IssTrackerData FetchIssTrackerData()
		{
			var existingData = this.DatabaseHandler.Get<IssTrackerData>();
			if (existingData != null && (DateTime.Now - existingData.TimeStamp) < this.TimerInterval)
			{
				return existingData;
			}

			var apiResponse = this.ApiHandler.Fetch(ApiType.IssTracker).Result;

			if (apiResponse is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "API Response is null");
				return IssTrackerData.Clone(existingData, errorStatus);
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "API Response data is empty");
				return IssTrackerData.Clone(existingData, errorStatus);
			}

			if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
			{
				var errorResult = ApiStatus.Failed(ApiType.IssTracker.ToString(), errorMessage);
				return IssTrackerData.Clone(existingData, errorResult);
			}

			IssTrackerJsonModel trackerData;
			try
			{
				trackerData = JsonConvert.DeserializeObject<IssTrackerJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "Exception while deserializing API response");
				return IssTrackerData.Clone(existingData, errorStatus);
			}

			if (string.IsNullOrWhiteSpace(trackerData?.ImagePath) || !File.Exists(trackerData.ImagePath))
			{
				var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "ISS Image does not exist on disk");
				return IssTrackerData.Clone(existingData, errorStatus);
			}

			var filename = Path.GetFileName(trackerData?.ImagePath);
			var relativePath = Path.Combine("images", filename);
			var destination = Path.Combine(this.WebHostEnvironment.WebRootPath, "images", filename);

			try
			{
				File.Copy(trackerData.ImagePath, destination, true);
			}
			catch (Exception ex)
			{
				var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), ex.Message);
				return IssTrackerData.Clone(existingData, errorStatus);
			}

			var status = ApiStatus.Success(ApiType.IssTracker.ToString(), DateTime.Now, ApiSource.Prod);
			return new IssTrackerData(status, relativePath, trackerData.Latitude, trackerData.Longitude, DateTime.Now);
		}
	}
}
