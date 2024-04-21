using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Transformers
{
    public class AutomataTransformer : TransformerBase
    {
        public AutomataTransformer(IApiHandler apiHandler) : base(apiHandler)
        {
        }

        public override IModuleViewModel Transform()
        {
            return null;
        }
    }
}
