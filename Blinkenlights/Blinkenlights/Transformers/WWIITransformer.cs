﻿using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.WWII;

namespace Blinkenlights.Transformers
{
    public class WWIITransformer : TransformerBase
    {
        IDataFetcher<WWIIData> DataFetcher { get; set; }

        public WWIITransformer(IApiHandler apiHandler, IDataFetcher<WWIIData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var data = this.DataFetcher.GetLocalData();
            if (data is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.WWII.ToString(), "Database lookup failed");
                return new WWIIDayModel(null, null, null, errorStatus);
            }

            var key = DateTime.Now.ToString("d MMM yyyy");
            if (data?.Days?.TryGetValue(key, out var currentDayData) != true || currentDayData?.GlobalEvents?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed(ApiType.WWII.ToString(), "No data available for today");
                return new WWIIDayModel(null, null, null, errorStatus);
            }

            return new WWIIDayModel(currentDayData.Date, currentDayData.GlobalEvents, currentDayData.RegionalEvents, data.Status);
        }
    }
}
