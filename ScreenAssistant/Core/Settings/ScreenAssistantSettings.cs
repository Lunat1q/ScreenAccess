using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using TiqUtils.SettingsController;

namespace TiqSoft.ScreenAssistant.Core.Settings
{
    internal class ScreenAssistantSettings
    {
        private static readonly SettingsController<ScreenAssistantSettings> SettingsController;
        internal static ScreenAssistantSettings Settings => SettingsController.Settings;

        static ScreenAssistantSettings()
        {
            SettingsController = new SettingsController<ScreenAssistantSettings>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        [DefaultValue(2)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int DeltaX { get; set; } = 2;

        [DefaultValue(3)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int DeltaY { get; set; } = 3;

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UseUniqueWeaponLogic { get; set; } = true;

        public void Save()
        {
            if (Settings == this)
            {
                SettingsController.Save();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
