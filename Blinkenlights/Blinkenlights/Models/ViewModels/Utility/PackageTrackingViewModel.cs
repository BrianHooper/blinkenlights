namespace Blinkenlights.Models.ViewModels.Utility
{
    public class Package
    {
        public string Name { get; set; }

        public string Carrier { get; set; }

        public string Status { get; set; }

        public string Eta { get; set; }

        public string Url { get; set; }

        public string Location { get; set; }

        public string Icon { get; set; }

        public Package() { }

        public Package(string name, string carrier, string status, string eta, string url, string location)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "-" : name;
            Carrier = string.IsNullOrWhiteSpace(carrier) ? "-" : carrier;
            Status = string.IsNullOrWhiteSpace(status) ? "-" : status; ;
            Eta = string.IsNullOrWhiteSpace(eta) ? "-" : eta;
            Url = string.IsNullOrWhiteSpace(url) ? "-" : url;
            Location = string.IsNullOrWhiteSpace(location) ? "-" : location;
            Icon = GetIcon(carrier);
        }

        private string GetIcon(string icon)
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

        public static Package FromResponse(KeyValuePair<string, Ship24Response> kv, IEnumerable<LiteDbLibrary.Schemas.PackageTrackingItem> trackingRequests)
        {
            var ship24Response = kv.Value?.Data;
            var trackingResponse = ship24Response?.Trackings?.FirstOrDefault();
            if (trackingResponse == null)
            {
                return null;
            }

            var request = trackingRequests?.FirstOrDefault(t => string.Equals(t?.TrackingNumber, kv.Key));
            if (request == null)
            {
                return null;
            }

            var lastEvent = trackingResponse.Events?.FirstOrDefault();

            var name = request.Name;
            var carrier = request.Carrier;
            var status = trackingResponse.Shipment?.StatusMilestone;
            var eta = trackingResponse.Shipment?.Delivery?.EstimatedDeliveryDate;
            var url = request.Url;
            var location = lastEvent?.Location;

            return new Package(name, carrier, status, eta, url, location);
        }
    }

    public class PackageTrackingViewModel
    {
        public List<Package> Packages { get; set; }

        public PackageTrackingViewModel(List<Package> packages = null)
        {
            this.Packages = packages;
        }
    }
}
