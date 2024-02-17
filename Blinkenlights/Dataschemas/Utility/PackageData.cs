namespace Blinkenlights.Dataschemas
{
    public class PackageData
    {
        public Package Package { get; set; }

        public string Carrier { get; set; }

        public string Status { get; set; }

        public string Eta { get; set; }

        public string Location { get; set; }

        public string Icon { get; set; }

        public ApiStatus ApiStatus { get; set; }

        public PackageData() { }

        public static PackageData Clone(Package package, PackageData other, ApiStatus apiStatus)
        {
            return new PackageData()
            {
                Package = package,
                ApiStatus = apiStatus,
                Carrier = other?.Carrier,
                Status = other?.Status,
                Eta = other?.Eta,
                Icon = other?.Icon
            };
        }
    }
}
