// Decompiled with JetBrains decompiler
// Type: CharacterCreation.Patches.CharacterObjectPatch
// Assembly: CharacterCreation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7DC7F99F-7C6D-44E1-9074-F7A2335153A6
// Assembly location: D:\Games\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\zzCharacterCreation\bin\Win64_Shipping_Client\CharacterCreation.dll

using System;
using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
using Helpers;
using HonestCharacterAge.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace HonestCharacterAge
{
    internal class CharacterObjectPatch
    {
        [HarmonyPatch(typeof(CharacterObject), "UpdatePlayerCharacterBodyProperties")]
        public class UpdatePlayerCharacterBodyProperties
        {
            private static bool Prefix(
                CharacterObject __instance,
                BodyProperties properties,
                ref bool isFemale)
            {
                try
                {
                    PropertyInfo property = typeof(Hero).GetProperty("StaticBodyProperties",
                        BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (__instance.IsHero)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(
                            "Hero updated: " + __instance.HeroObject.Name, Color.FromUint(4282569842U)));
                        property.SetValue(__instance.HeroObject, properties.StaticProperties);
                        __instance.HeroObject.DynamicBodyProperties = properties.DynamicProperties;
                        __instance.HeroObject.UpdatePlayerGender(isFemale);
                        float age = properties.DynamicProperties.Age;
                        if (age > HonestAgeHelper.MinimumAgeToApply)
                        {
                            age = HonestAgeHelper.OldToNew(age);
                        }
                        __instance.HeroObject.BirthDay = HeroHelper.GetRandomBirthDayForAge(age);
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An exception occurred whilst trying to apply the changes.\n\nException:\n" +
                                    ex.Message + "\n\n" + ex.InnerException?.Message);
                }

                return true;
            }
        }
    }
}