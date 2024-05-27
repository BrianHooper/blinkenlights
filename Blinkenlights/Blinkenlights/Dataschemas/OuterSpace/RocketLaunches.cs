namespace Blinkenlights.Dataschemas
{
	public class RocketLaunches
	{
		public List<RocketLaunch> Launches { get; set; }

		public ApiStatus Status { get; set; }

		public static RocketLaunches Clone(RocketLaunches other, ApiStatus status)
		{
			return new RocketLaunches()
			{
				Launches = other?.Launches,
				Status = status
			};
		}
	}

	public class RocketLaunch
	{
		public string Title { get; set; }

		public string Url { get; set; }

		public string Provider { get; set; }

		public string Name { get; set; }

		public string LaunchTime { get; set; }

		public string Location { get; set; }


	}
}
