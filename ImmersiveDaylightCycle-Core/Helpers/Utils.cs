using BepInEx.Logging;
using Comfort.Common;
using EFT;
using ImmersiveDaylightCycle.Common;
using ImmersiveDaylightCycle.Fika;
using Newtonsoft.Json;
using SPT.Common.Http;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jehree.ImmersiveDaylightCycle.Helpers
{
    internal class Utils
    {
        public static string TimeRequestURL = "/jehree/idc/request_time";
        public static string HostRaidStartedURL = "/jehree/idc/host_raid_started";
        public static string ClientLeftRaidURL = "/jehree/idc/client_exited";
        public static string ConsoleCommandURL = "/jehree/idc/console_command";



        public static bool IsDayTime(DateTime dateTime)
        {
            if (dateTime.Hour > 5 && dateTime.Hour < 21)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DisableTimeUI(TextMeshProUGUI phaseToDisable, Toggle timeToggle)
        {
            try
            {
                timeToggle.isOn = false;
                timeToggle.enabled = false;
                phaseToDisable.transform.gameObject.SetActive(false);
                var bg = timeToggle.transform.Find("Background").gameObject;
                bg.GetComponent<Image>().color = Color.clear;
                bg.transform.Find("Checkmark").gameObject.GetComponent<Image>().color = Color.clear;
            }
            catch (Exception) { }
        }

        public static void EnableTimeUI(TextMeshProUGUI phaseToEnable, Toggle timeToggle, string enabledText, bool chooseThisTime = true)
        {
            try
            {
                timeToggle.enabled = true;
                if (chooseThisTime) timeToggle.isOn = true;
                phaseToEnable.transform.gameObject.SetActive(true);
                var bg = timeToggle.transform.Find("Background").gameObject;
                bg.GetComponent<Image>().color = Color.white;
                bg.transform.Find("Checkmark").gameObject.GetComponent<Image>().color = Color.white;
                phaseToEnable.text = enabledText;
            }
            catch (Exception) { }
        }


        public static void SetRaidTime()
        {
            if (!Singleton<GameWorld>.Instantiated)
            {
                throw new Exception("Utils.SetRaidTime was called when the GameWorld instance was not yet instantiated!");
            }

            IDCTime time = ServerRoute<IDCTime>(Utils.TimeRequestURL);

            DateTime dateTime = new DateTime(2024, 6, 8, time.Hour, time.Minute, time.Second);
            Plugin.logger.LogError($"Setting Raid Time: '{dateTime}'");


            Singleton<GameWorld>.Instance.GameDateTime.Reset(DateTime.Now, dateTime, time.CycleRate);
        }

        public static DateTime GetCurrentTime()
        {
            IDCTime time = ServerRoute<IDCTime>(Utils.TimeRequestURL);
            return new DateTime(2024, 6, 8, time.Hour, time.Minute, time.Second);
        }

        public static float GetTimeCycleSpeed()
        {
            IDCTime time = ServerRoute<IDCTime>(Utils.TimeRequestURL);
            return time.CycleRate;
        }


        public static T ServerRoute<T>(string url, object data = default(object))
        {
            string json = JsonConvert.SerializeObject(data);
            var req = RequestHandler.PostJson(url, json);
            return JsonConvert.DeserializeObject<T>(req);
        }

        public static string ServerRoute(string url, object data = null)
        {
            string json = JsonConvert.SerializeObject(data);
            Plugin.logger.LogError($"Server Route: '{json}'");
            return RequestHandler.PutJson(url, json);
        }
    }
}
