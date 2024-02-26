namespace Blinkenlights.Dataschemas
{
	public class PeopleInSpace
	{
		public List<PersonInSpace> People { get; set; }

		public ApiStatus Status { get; set; }

		public static PeopleInSpace Clone(PeopleInSpace other, ApiStatus status)
		{
			return new PeopleInSpace()
			{
				People = other?.People,
				Status = status
			};
		}
	}

	public class PersonInSpace
	{
		public string Name { get; set; }

		public string Craft { get; set; }
	}
}
