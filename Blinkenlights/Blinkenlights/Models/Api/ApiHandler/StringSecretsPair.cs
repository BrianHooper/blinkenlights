using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public class StringSecretsPair
    {
        public string StringFormat { get; init; }

        public ApiSecretType[] SecretKeys { get; init; }

        public StringSecretsPair(string stringFormat, params ApiSecretType[] secretKeys)
        {
            StringFormat = stringFormat;
            SecretKeys = secretKeys;
        }

        public StringSecretsPair(params ApiSecretType[] secretKeys)
        {
            StringFormat = "{0}";
            SecretKeys = secretKeys;
        }
    }
}
