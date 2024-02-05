using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using LiteDbLibrary.Schemas;

namespace Blinkenlights.Models.Module
{
	public interface IModuleTypeData
	{
		public string Name { get; }

		public string Endpoint { get; }

		public int RefreshRateMs { get; }
	}

	public abstract class ModuleTypeData : IModuleTypeData
	{
		public virtual string Name => null;

		public virtual string Endpoint => null;

		public virtual int RefreshRateMs => 0;
	}

	public class ModuleTypeDataAttribute : Attribute
	{
		public IModuleTypeData Data { get; init; }

		public ModuleTypeDataAttribute(Type type)
		{
			Data = Activator.CreateInstance(type) as IModuleTypeData;
		}
	}

	public class TimeModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Time.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class WeatherModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Weather.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class IssTrackerModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.IssTracker.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class WWIIModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.WWII.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class HeadlinesModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Headlines.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class StockModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Stock.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class CalendarModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Calendar.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class UtilityModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Utility.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class Life360ModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Life360.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}

	public class SlideshowModuleData : ModuleTypeData
	{
		public override string Name => ModuleType.Slideshow.ToString();

		public override string Endpoint { get; } = "/Modules/GetTimeModule";

		public override int RefreshRateMs { get; } = 60 * 1000;
	}
}
