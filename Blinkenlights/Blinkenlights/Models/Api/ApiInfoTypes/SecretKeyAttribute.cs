namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public class SecretKeyAttribute : Attribute
    {
        public string SecretKey { get; set; }

        // TODO Make this nullable instead of returning unknown
        public ApiType SecretSource { get; set; }

        public SecretKeyAttribute(string key, ApiType source = ApiType.Unknown)
        {
            SecretKey = key;
            SecretSource = source;
        }
    }
}
