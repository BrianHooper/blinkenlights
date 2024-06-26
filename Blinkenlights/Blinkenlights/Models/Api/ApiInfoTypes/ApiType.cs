﻿namespace Blinkenlights.Models.Api.ApiInfoTypes
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
        Astronomy = 11,

        [ApiInfo(typeof(UpsPackageTrackingApiInfo))]
        UpsPackageTracking = 12,

        [ApiInfo(typeof(UpsOAuthApiInfo))]
        UpsOath = 13,

        [ApiInfo(typeof(PackageTrackingApiInfo))]
        PackageTracking = 14,

        [ApiInfo(typeof(Ship24ApiInfo))]
        Ship24 = 15,

        [ApiInfo(typeof(IssTrackerApiInfo))]
        IssTracker = 16,

		[ApiInfo(typeof(AlphaVantageApiInfo))]
		AlphaVantage = 17,

        [ApiInfo(typeof(WikiPotdApiInfo))]
        WikiPotd = 18,

        //[ApiInfo(typeof(DistanceApiInfo))]
        //Distance = 19,

        //[ApiInfo(typeof(FlightAwareApiInfo))]
        //FlightAware = 20,

		[ApiInfo(typeof(PeopleInSpaceApiInfo))]
		PeopleInSpace = 21,

		[ApiInfo(typeof(AlphaVantageCurrencyApiInfo))]
		AlphaVantageCurrency = 22,

		[ApiInfo(typeof(RocketLaunchLiveApiInfo))]
		RocketLaunchLive = 23,

        [ApiInfo(typeof(FlightRadarApiInfo))]
        FlightRadar = 24,

        [ApiInfo(typeof(BTownApiInfo))]
        BTown = 25,

        [ApiInfo(typeof(NprApiInfo))]
        Npr = 26,

        [ApiInfo(typeof(APNewsApiInfo))]
        APNews = 27,

        [ApiInfo(typeof(SeattleTimesApiInfo))]
        SeattleTimes = 28
    }
}
