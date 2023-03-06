namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public enum ApiType
    {
        Unknown = 0,

        [ApiInfo(typeof(TimeZoneApiInfo))]
        TimeZone = 1,

        [ApiInfo(typeof(MehApiInfo))]
        Meh = 2,

        [ApiInfo(typeof(Life360ApiInfo))]
        Life360 = 3,

        [ApiInfo(typeof(WWIIApiInfo))]
        WWII = 4,

        [ApiInfo(typeof(WikipediaApiInfo))]
        Wikipedia = 5,

        [ApiInfo(typeof(NewYorkTimesApiInfo))]
        NewYorkTimes = 6,

        [ApiInfo(typeof(GoogleCalendarApiInfo))]
        GoogleCalendar = 7,

        [ApiInfo(typeof(WeatherApiInfo))]
        VisualCrossingWeather = 8,

        [ApiInfo(typeof(YCombinatorApiInfo))]
        YCombinator = 9,

		[ApiInfo(typeof(RocketLaunchesApiInfo))]
		RocketLaunches = 10,

		[ApiInfo(typeof(AstronomyApiInfo))]
		Astronomy = 11
	}
}
