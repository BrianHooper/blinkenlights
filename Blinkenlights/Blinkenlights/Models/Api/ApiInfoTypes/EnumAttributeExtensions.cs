using Blinkenlights.Models.Module;
using LiteDbLibrary.Schemas;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public static class EnumAttributeExtensions
    {
        public static string GetSecretString(this ApiSecretType val)
        {
            var attribute = val.GetAttribute<SecretKeyAttribute>();
            return attribute is null ? string.Empty : attribute.SecretKey;
        }

        public static ApiType GetSecretSource(this ApiSecretType val)
        {
            var attribute = val.GetAttribute<SecretKeyAttribute>();
            return attribute is null ? ApiType.Unknown : attribute.SecretSource;
        }

        public static IApiInfo Info(this ApiType val)
        {
            return val.GetAttribute<ApiInfoAttribute>()?.ApiInfo;
        }

        public static IModuleTypeData Info(this ModuleType val)
        {
            return val.GetAttribute<ModuleTypeDataAttribute>()?.Data;
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
