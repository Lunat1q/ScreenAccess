using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TiqSoft.ScreenAssistant.Properties;
using TiqUtils.SettingsController;

namespace TiqLauncher.ScreenAssistant
{
    public sealed class SwappedStringsInfo
    {
        private static readonly SettingsController<SwappedStringsInfo> SettingsController;

        static SwappedStringsInfo()
        {
            SettingsController = new SettingsController<SwappedStringsInfo>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            {
                ConfigFileName = "FuzzyStrings.data"
            };
        }

        public FuzzyString AssemblyTitle { get; set; } = new FuzzyString(FuzzyStrings.AssemblyTitle);

        public FuzzyString AssemblyDescription { get; set; } = new FuzzyString(FuzzyStrings.AssemblyDescription);

        public FuzzyString AssemblyDeveloper { get; set; } = new FuzzyString(FuzzyStrings.AssemblyDeveloper);

        public FuzzyString AssemblyProduct { get; set; } = new FuzzyString(FuzzyStrings.AssemblyProduct);

        public IList<FuzzyString> GetStrings()
        {
            return new[]
            {
                this.AssemblyTitle, this.AssemblyDescription, this.AssemblyDeveloper, this.AssemblyProduct
            };
        }

        public static SwappedStringsInfo Load()
        {
            return SettingsController.Settings;
        }

        public void Save()
        {
            SettingsController.Save();
        }
    }

    public sealed class FuzzyString
    {
        public string StringData { get; set; }

        public int Index { get; set; } = -1;

        public FuzzyString(string stringData)
        {
            this.StringData = stringData;
        }

        public void MakeItFuzzy()
        {
            this.StringData = Program.RandomString(this.StringData.Length);
        }
    }
}
