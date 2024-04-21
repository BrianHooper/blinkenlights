using Blinkenlights.ApiHandlers;
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
        public UtilityDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<UtilityDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

		protected override UtilityData GetRemoteData(UtilityData existingData = null, bool overwrite = false)
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
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.Meh.Info()))
            {
                return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.Meh} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.Meh);
            if (response is null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Meh, "Api response is null");
                return MehData.Clone(existingData, errorStatus);
            }
            else if (string.IsNullOrWhiteSpace(response.Data))
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Meh, "Api response is empty", response.LastUpdateTime);
                return MehData.Clone(existingData, errorStatus);
            }

            MehJsonModel model;
            try
            {
                model = JsonSerializer.Deserialize<MehJsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Meh, "Error deserializing api response", response.LastUpdateTime);
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
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Meh, "Required data missing in api response", response.LastUpdateTime);
                return MehData.Clone(existingData, errorStatus);
            }

            var apiStatus = this.ApiStatusFactory.Success(ApiType.Meh, response.LastUpdateTime, response.ApiSource);
            return new MehData($"Meh - ${price} - {title}", url, imageUrl, apiStatus);
        }

        private List<Package> GetPackages()
        {
            // TODO load from external source
            return new List<Package>()
            {
                new Package()
                {
                    Name = "Hose Reel",
                    TrackingNumber = "775761253501",
                    Url = "https://www.fedex.com/fedextrack/?trknbr=775761253501&trkqual=12027~775761253501~FDEG",
                    Carrier = "fedex"
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
				return existingPackageData;
			}

			this.Logger.LogInformation($"Calling {ApiType.Ship24} remote API");
			var apiRequestData = new Ship24Request(package.TrackingNumber);
            var serializedRequest = JsonSerializer.Serialize(apiRequestData);
            var response = await this.ApiHandler.Fetch(ApiType.Ship24, serializedRequest);
            if (response == null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Ship24, "Api response was null");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            if (response.ResultStatus != ApiResultStatus.Success)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Ship24, $"Api failed with response {response.ResultStatus}");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(response.Data))
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Ship24, $"Api response was empty");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            var ship24Response = Ship24Response.Deserialize(response.Data);
            if (ship24Response == null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Ship24, $"Failed to deserialize api response");
                return PackageData.Clone(package, existingPackageData, errorStatus);
            }

            var trackingResponse = ship24Response?.Data?.Trackings?.FirstOrDefault();
            if (trackingResponse == null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Ship24, $"No trackings in api response");
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
                ApiStatus = this.ApiStatusFactory.Success(ApiType.Ship24, DateTime.Now, ApiSource.Prod),
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
                "fedex" => Path.Combine("images", "packagetracking", "fedex.png"),
                _ => null
            };
        }
    }
}
