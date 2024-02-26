namespace Blinkenlights.Dataschemas
{
    public class CurrentCondition
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

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