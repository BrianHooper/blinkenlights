namespace Blinkenlights.Dataschemas
{
    public interface IModuleData
    {
        public string Key();

        public string Value();

        public DateTime? TimeStamp { get; init; }
    }
}