using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class ModuleDataResponse
    {
        public List<IModuleData> ModuleData { get; init; }

        public ModuleDataResponse(List<IModuleData> moduleData)
        {
            ModuleData = moduleData;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static ModuleDataResponse? Deserialize(string json)
        {
            try
            {
                var data = JsonSerializer.Deserialize<ModuleDataResponse>(json);
                return data;
            }
            catch 
            { 
                return null;
            }
        }
    }
}
