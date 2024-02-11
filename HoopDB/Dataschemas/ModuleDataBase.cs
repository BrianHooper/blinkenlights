namespace Blinkenlights.Dataschemas
{
	public abstract class ModuleDataBase : IModuleData
	{
		public virtual int Id { get; set; }

		public virtual bool SameIndex(IModuleData other) => this.Equals(other);
	}
}
