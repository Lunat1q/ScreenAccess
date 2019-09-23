using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TiqSoft.ScreenAssistant.Annotations;
using TiqSoft.ScreenAssistant.Builders;
using TiqUtils.SettingsController;
using TiqUtils.Wpf.UIBuilders;

namespace TiqSoft.ScreenAssistant.Core.Settings
{
    [DisplayName("Settings"), UIBuilder(nameof(ScreenAssistantSettingsBuilder))]
    internal class ScreenAssistantSettings : INotifyPropertyChanged
    {
        #region Groups
        private const string RecoilGroup = "Recoil settings";
        private const string RecognitionGroup = "Recognition settings";
        #endregion

        private static readonly SettingsController<ScreenAssistantSettings> SettingsController;
        internal static ScreenAssistantSettings Settings => SettingsController.Settings;

        private float _sensitivityScale = 1;
        private float _brightnessScale = 1;
        private bool _useUniqueWeaponLogic = true;
        private bool _lockToGameWindow = true;
        private bool _fullScreenMode;
        private string _selectedGameName = "Apex Legends";

        static ScreenAssistantSettings()
        {
            SettingsController = new SettingsController<ScreenAssistantSettings>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }
        

        [PropertyMember, PropertyGroup(RecoilGroup)]
        [DisplayName("Sensitivity adjustment")]
        [SliderLimits(0.1f, 5)]
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float SensitivityScale
        {
            get => this._sensitivityScale;
            set
            {
                if (value.Equals(this._sensitivityScale)) return;
                this._sensitivityScale = value;
                this.OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecognitionGroup)]
        [DisplayName("Brightness adjustment")]
        [SliderLimits(0.3f, 1.18f, 2, 0.01f, 0.01f)]
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float BrightnessScale
        {
            get => this._brightnessScale;
            set
            {
                if (value.Equals(this._brightnessScale)) return;
                this._brightnessScale = value;
                this.OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecoilGroup)]
        [DisplayName("Unique weapon logic")]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UseUniqueWeaponLogic
        {
            get => this._useUniqueWeaponLogic;
            set
            {
                if (value == this._useUniqueWeaponLogic) return;
                this._useUniqueWeaponLogic = value;
                this.OnPropertyChanged();
            }
        }

        [PropertyMember]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool LockToGameWindow
        {
            get => this._lockToGameWindow;
            set
            {
                if (value == this._lockToGameWindow) return;
                this._lockToGameWindow = value;
                this.OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecognitionGroup)]
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool FullScreenMode
        {
            get => this._fullScreenMode;
            set
            {
                if (value == this._fullScreenMode) return;
                this._fullScreenMode = value;
                this.OnPropertyChanged();
            }
        }

        [PropertyMember]
        [DisplayName("Select the game:")]
        [DefaultValue("Apex Legends")]
        [PropertyOrder(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string SelectedGameName
        {
            get => this._selectedGameName;
            set
            {
                if (value == this._selectedGameName) return;
                this._selectedGameName = value;
                this.OnPropertyChanged();
            }
        }

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


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
