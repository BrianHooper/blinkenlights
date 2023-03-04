using System.ComponentModel;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public enum ApiSecretType
    {
        Default = 0,
        [Description("Life360:AuthorizationToken")]
        Life360AuthorizationToken = 1,
        [Description("Life360:CircleId")]
        Life360CircleId = 2,
        [Description("VisualCrossing:ServiceApiKey")]
        VisualCrossingServiceApiKey = 3,
        [Description("Meh:ServiceApiKey")]
        MehServiceApiKey = 4,
        [Description("NewYorkTimes:ServiceApiKey")]
        NewYorkTimesServiceApiKey = 5,
        [Description("GoogleCalendar:UserAccount")]
        GoogleCalendarUserAccount = 6,
        [Description("GoogleCalendar:ApiServiceKey")]
        GoogleCalendarApiServiceKey = 7,
    }
}
