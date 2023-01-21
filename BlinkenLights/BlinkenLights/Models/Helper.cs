namespace BlinkenLights.Models
{
    public class Helper
    {
        public static DateTime FromEpoch(long epoch, bool useUtc = false, bool addOffset = false)
        {
            var dtKind = useUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, dtKind).AddSeconds(epoch);

            if (addOffset)
            {
                dt.Add(DateTimeOffset.Now.Offset);
            }
            return dt;
        }
    }
}
