namespace Blinkenlights.Dataschemas
{
	public class RocketLaunchLiveData
	{
		public ApiStatus Status { get; set; }

		public static RocketLaunchLiveData Clone(RocketLaunchLiveData other, ApiStatus status)
		{
			return new RocketLaunchLiveData()
			{
				Status = status,
			};
		}
	}
}
