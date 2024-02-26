namespace Blinkenlights.Dataschemas
{
    public interface IDatabaseData
    {
        public string Key();

        public string Value();

        public DateTime? TimeStamp { get; init; }
    }
}