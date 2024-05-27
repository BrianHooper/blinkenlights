using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;

namespace Blinkenlights.Transformers
{
    public class CalendarTransformer : TransformerBase
    {
        private IDataFetcher<CalendarModuleData> dataFetcher { get; init; }

        public CalendarTransformer(IApiHandler apiHandler, IDataFetcher<CalendarModuleData> dataFetcher) : base(apiHandler)
        {
            this.dataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var calendarDataRsp = this.dataFetcher.FetchRemoteData();

            if (calendarDataRsp == null)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, "Database lookup failed");
                return new CalendarViewModel();
            }

            if (calendarDataRsp.Events?.Any() != true)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, "No events in local database");
                return new CalendarViewModel();
            }

            return new CalendarViewModel(calendarDataRsp.Events, calendarDataRsp.Status);
        }
    }
}
