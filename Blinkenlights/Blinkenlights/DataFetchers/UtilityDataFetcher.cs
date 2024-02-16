using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.Utility;
using Newtonsoft.Json;
using System.Threading;

namespace Blinkenlights.DataFetchers
{
    public class UtilityDataFetcher : DataFetcherBase<UtilityData>
    {
        public UtilityDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromHours(4), databaseHandler, apiHandler)
        {
            Start();
        }

        protected override UtilityData GetRemoteData(UtilityData existingData = null)
        {
            var mehData = GetMehData(existingData?.MehData);
            var packageTrakingData = GetPackageTrackingData(existingData?.PackageTrackingData);
            return new UtilityData()
            {
                MehData = mehData.Result,
                PackageTrackingData = packageTrakingData
            };
        }

        private async Task<MehData> GetMehData(MehData existingData)
        {
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
                model = JsonConvert.DeserializeObject<MehJsonModel>(response.Data);
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


        private PackageTrackingData GetPackageTrackingData(PackageTrackingData existingData)
        {
            // TODO load from external source
            var packages = new List<Package>()
            {
                new Package()
                {
                    Name = "Present",
                    TrackingNumber = "9434608205499799759287",
                    Url = "https://tools.usps.com/go/TrackConfirmAction_input?strOrigTrackNum=9434608205499799759287"
                }
            };

            var packageDataResponses = packages.Select(pkg => TrackPackage(pkg, existingData));
            return new PackageTrackingData()
            {
                Packages = packageDataResponses.Select(pkg => pkg.Result).ToList()
            };
        }

        private async Task<PackageData> TrackPackage(Package package, PackageTrackingData existingData)
        {
            var existingPackageData = existingData?.Packages?.FirstOrDefault(pkg => string.Equals(pkg?.Package?.TrackingNumber, package.TrackingNumber, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(package?.TrackingNumber))
            {
                return null;
            }

            var apiRequestData = new Ship24Request(package.TrackingNumber);
            var serializedRequest = JsonConvert.SerializeObject(apiRequestData);
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

            var lastEvent = trackingResponse.Events?.FirstOrDefault();
            var status = trackingResponse.Shipment?.StatusMilestone;
            var eta = trackingResponse.Shipment?.Delivery?.EstimatedDeliveryDate;
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
