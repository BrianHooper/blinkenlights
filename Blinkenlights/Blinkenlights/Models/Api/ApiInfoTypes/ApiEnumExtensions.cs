using System.ComponentModel;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public static class ApiEnumExtensions
    {
        public static string ToSecretString(this ApiSecretType val)
        {
            var attribute = val.GetAttribute<DescriptionAttribute>();
            return attribute is null ? string.Empty : attribute.Description;
        }

        public static IApiInfo Info(this ApiType val)
        {
            return val.GetAttribute<ApiInfoAttribute>()?.ApiInfo;
        }

        private static T GetAttribute<T>(this object val) where T : Attribute
        {
            var attribute = val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(T), false)
               .ToList()
               .FirstOrDefault();

            return attribute as T;
        }
    }
}
