using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using HarmonyLib;
using HonestCharacterAge.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace HonestCharacterAge
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony harmony;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            HonestAgeHelper.Load("HonestCharacterAge");
            harmony = new Harmony("mod.bannerleiros.honestcharacterage");
            Harmony.DEBUG = true;
            PatchAge();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage(
                "HonestCharacterAge loaded!!",
                Color.FromUint(Convert.ToUInt32("0x92c3fb", 16))));

        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
            if (HonestAgeHelper.IsOverwritten) return;
            try
            {
                
                Task task = new Task(async () =>
                {
                    
                    while (Campaign.Current.Heroes == null)
                    {
                        await Task.Delay(1000);
                    }
                    
                    OverrideCharactersAgeAppearance();
                });
                
                task.Start();
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
        
        public async void OverrideCharactersAgeAppearance()
        {
            var heroes = Campaign.Current.Heroes;
            InformationManager.DisplayMessage(new InformationMessage("Starting Override!", Colors.Magenta));
            
            if (heroes == null || heroes.Count <= 0)
            {
                InformationManager.DisplayMessage(new InformationMessage("Heroes not loaded!", Colors.Red));
                return;
                
            }
            foreach (var hero in heroes)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Modifying Hero {hero.Name}!", Colors.Cyan));
                if (hero.Age < HonestAgeHelper.MinimumHeroAge)
                {
                    await Task.Delay(5);
                    continue;
                }
                var age = HonestAgeHelper.NewToOld(hero.Age);
                DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(age,
                    hero.DynamicBodyProperties.Build, hero.DynamicBodyProperties.Weight);
                BodyProperties bodyProperties =
                    new BodyProperties(dynamicBodyProperties, hero.BodyProperties.StaticProperties);
                hero.CharacterObject?.UpdatePlayerCharacterBodyProperties(bodyProperties, hero.IsFemale);
                await Task.Delay(5);
            }

            HonestAgeHelper.SaveOverwritten("HonestCharacterAge");
            InformationManager.DisplayMessage(new InformationMessage("Modification Finished!", Colors.Blue));
        }

        public void PatchAge()
        {
            try
            {
                var original = typeof(CharacterObject).GetMethod("UpdatePlayerCharacterBodyProperties");
                harmony.Unpatch(original,HarmonyPatchType.Prefix,"mod.bannerlord.popowanobi.dcc");
                Assembly assembly = Assembly.GetExecutingAssembly();
                harmony.PatchAll(assembly);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            } 
        }
    }
}