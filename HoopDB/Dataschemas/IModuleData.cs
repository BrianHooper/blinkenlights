namespace Blinkenlights.Dataschemas
{
	public interface IModuleData
	{
		public int Id { get; set; }

		public bool SameIndex(IModuleData other);

	}
}
