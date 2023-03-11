namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public class ApiInfoAttribute : Attribute
    {
        public IApiInfo ApiInfo { get; init; }

        public ApiInfoAttribute(Type infoType)
        {
            ApiInfo = Activator.CreateInstance(infoType) as IApiInfo;
        }
    }
}
