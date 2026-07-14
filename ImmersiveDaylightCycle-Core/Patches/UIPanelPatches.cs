using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using Jehree.ImmersiveDaylightCycle.Helpers;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

namespace Jehree.ImmersiveDaylightCycle.Patches
{
    internal class TimeUIPanelPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocationConditionsPanel), nameof(LocationConditionsPanel.Set));
        }

        [PatchPostfix]
        static void Postfix(
            RaidSettings raidSettings,
            bool takeFromCurrent,
            ref TextMeshProUGUI ____currentPhaseTime,
            ref TextMeshProUGUI ____nextPhaseTime,
            ref Toggle ____pmTimeToggle,
            ref Toggle ____amTimeToggle
        )
        {
            if (!Settings.ModEnabled.Value)
            {
                Utils.EnableTimeUI(____nextPhaseTime, ____pmTimeToggle, "03:28:00", false);
                Utils.EnableTimeUI(____currentPhaseTime, ____amTimeToggle, "15:28:00", false);
                return;
            }

            DateTime dateTime = Utils.GetCurrentTime();

            if (raidSettings.SelectedLocation.Id == "factory4_day" || raidSettings.SelectedLocation.Id == "factory4_night")
            {

                if (Settings.FactoryTimeAlwaysSelectable.Value)
                {
                    Utils.EnableTimeUI(____nextPhaseTime, ____pmTimeToggle, $"NIGHT-{dateTime.ToString("HH")}", false);
                    Utils.EnableTimeUI(____currentPhaseTime, ____amTimeToggle, $"DAY-{dateTime.ToString("HH")}", false);
                    return;
                }

                if (Utils.IsDayTime(dateTime))
                {
                    Utils.DisableTimeUI(____nextPhaseTime, ____pmTimeToggle);
                    Utils.EnableTimeUI(____currentPhaseTime, ____amTimeToggle, $"DAY-{dateTime.ToString("HH")}", false);
                }
                else
                {
                    Utils.DisableTimeUI(____currentPhaseTime, ____amTimeToggle);
                    Utils.EnableTimeUI(____nextPhaseTime, ____pmTimeToggle, $"NIGHT-{dateTime.ToString("HH")}", false);
                }
                return;
            }

            Utils.DisableTimeUI(____nextPhaseTime, ____pmTimeToggle);
            Utils.EnableTimeUI(____currentPhaseTime, ____amTimeToggle, dateTime.ToString("HH:mm:ss"));
        }
    }

    internal class TimeUIUpdatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocationConditionsPanel), nameof(LocationConditionsPanel.Update));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            if (!Settings.ModEnabled.Value) return true;
            return false;
        }
    }

    internal class LocationConditionsPanelPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.FirstMethod(typeof(LocationConditionsPanel), x => x.Name == nameof(LocationConditionsPanel.Set) && x.GetParameters()[0].Name == "session");
        }

        [PatchPostfix]
        static void Postfix(RaidSettings raidSettings, bool takeFromCurrent, MatchMakerAcceptScreen __instance)
        {
            if (!Settings.ModEnabled.Value) return;

            DateTime dateTime = Utils.GetCurrentTime();

            TextMeshProUGUI timePanel;

            try
            {
                timePanel = __instance.transform.Find("TimePanel").gameObject.transform.Find("Time").gameObject.GetComponent<TextMeshProUGUI>();
            }
            catch (Exception) { return; }

            if (raidSettings.SelectedLocation.Id == "factory4_day" || raidSettings.SelectedLocation.Id == "factory4_night")
            {

                if (Settings.FactoryTimeAlwaysSelectable.Value) return;

                if (Utils.IsDayTime(dateTime))
                {
                    SetTimePanelText(timePanel, "15:28:00");
                }
                else
                {
                    SetTimePanelText(timePanel, "03:28:00");
                }
                return;
            }

            SetTimePanelText(timePanel, dateTime.ToString("HH:mm:ss"));
        }

        static void SetTimePanelText(TextMeshProUGUI timePanel, string text)
        {
            try
            {
                timePanel.text = text;
            }
            catch (Exception) { }
        }
    }
}
