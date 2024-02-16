namespace Blinkenlights.Dataschemas
{
    public class CurrentCondition
    {
        public string Key;

        public string Value;

        public string Icon;

        public string Description;

        public static CurrentCondition Create(string key, string value, string icon, string description)
        {
            return new CurrentCondition()
            {
                Key = key,
                Value = value,
                Icon = icon,
                Description = description
            };
        }
    }
}