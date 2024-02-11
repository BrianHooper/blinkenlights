using Blinkenlights.Dataschemas;
using RestSharp;
using System.Text.Json;

namespace DataIngestionPipeline
{
	public class Program
	{
		private string PUT_URL = "https://localhost:7290/write/{0}";

		private string GET_URL = "https://localhost:7290/read/{0}";


		private void Write(IModuleData data)
		{
			var client = new RestClient(string.Format(PUT_URL, data.Key()));
			var request = new RestRequest();
			request.AddQueryParameter("value", data.Value());
			var response = client.Put(request);
		}

		private string Read(string key)
		{
			var client = new RestClient(string.Format(GET_URL, key));
			var request = new RestRequest();
			var response = client.Get(request);
			return response.Content;
		}

		private void Run()
		{
			var issData = new IssTrackerData
			{
				FilePath = "C:/Users/brian/Documents/code/blinkenlights/PageParseApi/iss_location.png",
				Latitude = 29.3113,
				Longitude = 125.8582,
				TimeStamp = DateTime.Now,
				ApiResult = new ApiResult
				{
					Status = ApiStatusMsg.Success,
					TimeStamp = DateTime.Now,
					Message = "Success"
				}
			};

			Write(issData);

			var x = Read(issData.Key());
			var z = JsonSerializer.Deserialize<IssTrackerData>(x);
		}

		static void Main(string[] args)
		{
			new Program().Run();
		}
	}
}