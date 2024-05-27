using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Blinkenlights.Models.ViewModels.OuterSpace
{
	public class RocketLaunchLiveJsonModel
	{
		[JsonPropertyName("valid_auth")]
		public bool ValidAuth { get; set; }

		[JsonPropertyName("count")]
		public long Count { get; set; }

		[JsonPropertyName("limit")]
		public long Limit { get; set; }

		[JsonPropertyName("total")]
		public long Total { get; set; }

		[JsonPropertyName("last_page")]
		public long LastPage { get; set; }

		[JsonPropertyName("result")]
		public List<Result> Result { get; set; }
	}

	public class Result
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("cospar_id")]
		public string CosparId { get; set; }

		[JsonPropertyName("sort_date")]
		public string SortDate { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("provider")]
		public Provider Provider { get; set; }

		[JsonPropertyName("vehicle")]
		public Provider Vehicle { get; set; }

		[JsonPropertyName("pad")]
		public Pad Pad { get; set; }

		[JsonPropertyName("missions")]
		public List<Mission> Missions { get; set; }

		[JsonPropertyName("mission_description")]
		public string MissionDescription { get; set; }

		[JsonPropertyName("launch_description")]
		public string LaunchDescription { get; set; }

		[JsonPropertyName("win_open")]
		public object WinOpen { get; set; }

		[JsonPropertyName("t0")]
		public string T0 { get; set; }

		[JsonPropertyName("win_close")]
		public string WinClose { get; set; }

		[JsonPropertyName("est_date")]
		public EstDate EstDate { get; set; }

		[JsonPropertyName("date_str")]
		public string DateStr { get; set; }

		[JsonPropertyName("tags")]
		public List<Tag> Tags { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("weather_summary")]
		public object WeatherSummary { get; set; }

		[JsonPropertyName("weather_temp")]
		public object WeatherTemp { get; set; }

		[JsonPropertyName("weather_condition")]
		public object WeatherCondition { get; set; }

		[JsonPropertyName("weather_wind_mph")]
		public object WeatherWindMph { get; set; }

		[JsonPropertyName("weather_icon")]
		public object WeatherIcon { get; set; }

		[JsonPropertyName("weather_updated")]
		public object WeatherUpdated { get; set; }

		[JsonPropertyName("quicktext")]
		public string Quicktext { get; set; }

		[JsonPropertyName("media")]
		public List<object> Media { get; set; }

		[JsonPropertyName("result")]
		public long? ResultResult { get; set; }

		[JsonPropertyName("suborbital")]
		public bool Suborbital { get; set; }

		[JsonPropertyName("modified")]
		public DateTimeOffset Modified { get; set; }
	}

	public class EstDate
	{
		[JsonPropertyName("month")]
		public long Month { get; set; }

		[JsonPropertyName("day")]
		public long Day { get; set; }

		[JsonPropertyName("year")]
		public long Year { get; set; }

		[JsonPropertyName("quarter")]
		public object Quarter { get; set; }
	}

	public class Mission
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; }
	}

	public class Pad
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("location")]
		public Location Location { get; set; }
	}

	public class Location
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("state")]
		public string State { get; set; }

		[JsonPropertyName("statename")]
		public string Statename { get; set; }

		[JsonPropertyName("country")]
		public string Country { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }
	}

	public class Provider
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("company_id")]
		public long? CompanyId { get; set; }
	}

	public class Tag
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("text")]
		public string Text { get; set; }
	}
}
