using Newtonsoft.Json;

namespace Blinkenlights.Models.Api.ApiResult
{
	public class ApiError
    {
        public string Error { get; set; }

        public static bool IsApiError(string response, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(response))
            {
                return false;
            }

            try
            {
                var errorModel = JsonConvert.DeserializeObject<ApiError>(response);
                if (!string.IsNullOrWhiteSpace(errorModel?.Error))
                {
                    errorMessage = errorModel.Error;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
