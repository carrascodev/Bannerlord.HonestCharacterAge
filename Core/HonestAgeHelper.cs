using System.IO;
using Newtonsoft.Json.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace HonestCharacterAge.Core
{
    public class HonestAgeHelper
    {
        public static float MaxAge { get; private set; }
        public static bool IsOverwritten { get; private set; }

        public static float MinimumAgeToApply { get; private set; }

        public static float MinimumHeroAge => NewToOld(MinimumAgeToApply);

        private static float oldAge => DynamicBodyProperties.MaxAge;

        private static float newAgeScale => MaxAge / oldAge;
        private static float oldAgeScale => oldAge / MaxAge;

        public static float OldToNew(float age) => age * newAgeScale;

        public static float NewToOld(float age) => age * oldAgeScale;
        
        
        public static void Load(string moduleName)
        {
            string path = Path.Combine(BasePath.Name, "Modules", moduleName, "ModuleData/config.json");
            JObject obj = JObject.Parse(File.ReadAllText(path));
            MaxAge = (float)obj["maxAge"];
            MinimumAgeToApply = (float)obj["minimumAgeToApply"];
            IsOverwritten = (bool)obj["overwritten"];
        }


        public static void SaveOverwritten(string moduleName)
        {
            string path = Path.Combine(BasePath.Name, "Modules", moduleName, "ModuleData/config.json");
            JObject obj = new JObject(new JProperty("maxAge", MaxAge), new JProperty("overwritten", true), new JProperty("minimumAgeToApply", MinimumAgeToApply));
            File.WriteAllText(path, obj.ToString());
        }
    }
}