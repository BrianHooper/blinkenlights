using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.Utility;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class UtilityDataFetcher : DataFetcherBase<UtilityData>
    {
        public UtilityDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<UtilityDataFetcher> logger) : base(databaseHandler, apiHandler, logger)
        {
            Start();
        }

		public override UtilityData GetRemoteData(UtilityData existingData = null, bool overwrite = false)
        {
            var mehData = GetMehData(existingData?.MehData, overwrite);
            var packageTrakingData = GetPackageTrackingData(existingData?.PackageTrackingData, overwrite);

            return new UtilityData()
            {
                MehData = mehData.Result,
                PackageTrackingData = packageTrakingData,
                TimeStamp = DateTime.Now,
            };
        }

        private async Task<MehData> GetMehData(MehData existingData, bool overwrite)
        {
            //TODO Check if data is valid
            if (!overwrite && !IsExpired(existingData?.ApiStatus, ApiType.Meh.Info()))
            {
                return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.Meh} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.Meh);
            if (response is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh.ToString(), "Api response is null");
                return MehData.Clone(existingData, errorStatus);
            }
            else if (string.IsNullOrWhiteSpace(response.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh.ToString(), "Api response is empty", response.LastUpdateTime);
                return MehData.Clone(existingData, errorStatus);
            }

            MehJsonModel model;
            try
            {
                model = JsonSerializer.Deserialize<MehJsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh.ToString(), "Error deserializing api response", response.LastUpdateTime);
                return MehData.Clone(existingData, errorStatus);
            }

            var title = model?.Deal?.Title;
            var item = model?.Deal?.Items?.FirstOrDefault();
            var url = model?.Deal?.Url;
            var imageUrl = model?.Deal?.Photos?.FirstOrDefault(i => Path.GetExtension(i)?.ToLower()?.Equals(".gif") != true);
            var price = item?.Price.ToString();
            if (string.IsNullOrWhiteSpace(title)
                || string.IsNullOrWhiteSpace(url)
                || string.IsNullOrWhiteSpace(imageUrl)
                || string.IsNullOrWhiteSpace(price))
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh.ToString(), "Required data missing in api response", response.LastUpdateTime);
                return MehData.Clone(existingData, errorStatus);
            }

            var apiStatus = ApiStatus.Success(ApiType.Meh.ToString(), response.LastUpdateTime, response.ApiSource);
            return new MehData($"Meh - ${price} - {title}", url, imageUrl, apiStatus);
        }

        private List<Package> GetPackages()
        {
            // TODO load from external source
            return new List<Package>()
            {
                new Package()
                {
                    Name = "Present",
                    TrackingNumber = "9434608205499799759287",
                    Url = "https://tools.usps.com/go/TrackConfirmAction_input?strOrigTrackNum=9434608205499799759287",
                    Carrier = "usps"
                }
            };
        }

        private PackageTrackingData GetPackageTrackingData(PackageTrackingData existingData, bool overwrite)
        {
            var packages = GetPackages();
            if (packages?.Any() != true)
            {
                return new PackageTrackingData();
            }

			var packageDataResponses = packages.Select(pkg => TrackPackage(pkg, existingData, overwrite));
            return new PackageTrackingData()
            {
                Packages = packageDataResponses.Select(pkg => pkg.Result).ToList()
            };
        }

        private async Task<PackageData> TrackPackage(Package package, PackageTrackingData existingData, bool overwrite)
        {
            var existingPackageData = existingData?.Packages?.FirstOrDefault(pkg => string.Equals(pkg?.Package?.TrackingNumber, package.TrackingNumber, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(package?.TrackingNumber))
            {
                return null;
            }

			if (!overwrite && !IsExpired(existingPackageData?.ApiStatus, ApiType.Ship24.Info()))
			{
				this.Logger.LogDebug($"Using cached data for {ApiType.Ship24} API");
				return existingPackageData;
			}

			this.Logger.LogInformation($"Calling {ApiType.Ship24} remote API");
			var apiRequestData = new Ship24Request(package.TrackingNumber);
            var serializedRequest = JsonSerializer.Serialize(apiRequestData);
            var response = await this.ApiHandler.Fetch(ApiType.Ship24, serializedRequest);
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Ship24.ToString(), "Api response was null");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            if (response.ResultStatus != ApiResultStatus.Success)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Ship24.ToString(), $"Api failed with response {response.ResultStatus}");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(response.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.Ship24.ToString(), $"Api response was empty");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            var ship24Response = Ship24Response.Deserialize(response.Data);
            if (ship24Response == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Ship24.ToString(), $"Failed to deserialize api response");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            var trackingResponse = ship24Response?.Data?.Trackings?.FirstOrDefault();
            if (trackingResponse == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Ship24.ToString(), $"No trackings in api response");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            var eta = "-";
            if (trackingResponse.Shipment?.Delivery?.EstimatedDeliveryDate != null && DateTime.TryParse(trackingResponse.Shipment.Delivery.EstimatedDeliveryDate, out var etaDateTime)) 
            {
                eta = etaDateTime.ToShortDateString();
            }

            var lastEvent = trackingResponse.Events?.FirstOrDefault();
            var status = trackingResponse.Shipment?.StatusMilestone;
            var location = lastEvent?.Location;
            var icon = GetIcon(package.Carrier);

            return new PackageData()
            {
                Package = package,
                Status = status,
                Eta = eta,
                Location = location,
                Icon = icon,
                ApiStatus = ApiStatus.Success(ApiType.Ship24.ToString(), DateTime.Now, ApiSource.Prod),
            };
        }

        private static string GetIcon(string icon)
        {
            if (icon == null)
            {
                return null;
            }

            return icon.ToLowerInvariant() switch
            {
                "ups" => Path.Combine("images", "packagetracking", "ups.png"),
                "usps" => Path.Combine("images", "packagetracking", "usps.png"),
                _ => null
            };
        }
    }
}
