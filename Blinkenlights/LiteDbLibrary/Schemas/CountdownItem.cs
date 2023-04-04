namespace LiteDbLibrary.Schemas
{
	public class CountdownItem : ILiteDbObject
	{
		public DateTime Date { get; set; }

		public string Name { get; set; }

		public int Id { get; set; }

		public bool SameIndex(ILiteDbObject other)
		{
			return string.Equals(this.Name, (other as CountdownItem).Name);
		}
	}
}
