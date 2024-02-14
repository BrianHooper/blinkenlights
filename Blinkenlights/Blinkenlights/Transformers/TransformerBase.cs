using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Transformers
{
    public abstract class TransformerBase
    {
        protected IApiHandler ApiHandler { get; set; }

        protected TransformerBase(IApiHandler apiHandler)
        {
            ApiHandler = apiHandler;
        }

        public abstract IModuleViewModel Transform();
    }
}
