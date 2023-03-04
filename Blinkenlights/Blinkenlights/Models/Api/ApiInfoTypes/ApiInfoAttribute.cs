namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public class ApiInfoAttribute : Attribute
    {
        public IApiInfo ApiInfo { get; init; }

        public ApiInfoAttribute(Type type)
        {
            ApiInfo = Activator.CreateInstance(type) as IApiInfo;
        }
    }
}
