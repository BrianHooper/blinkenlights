namespace BlinkenLights.Transformers
{
    public class CalendarTransformer
    {
        public static bool TryGetCalendarViewModel(string icsResponse)
        {
            if (string.IsNullOrWhiteSpace(icsResponse))
            {
                return false;
            }

            string[] lines = icsResponse.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );
            foreach(var line in lines)
            {

            }

            return false;
        }
    }
}
