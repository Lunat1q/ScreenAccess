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

        private int _deltaX = 2;
        private int _deltaY = 3;
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
        [DisplayName("Average X Offset")]
        [SliderLimits(1, 10, 0, 1, 1)]
        [DefaultValue(2)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int DeltaX
        {
            get => _deltaX;
            set
            {
                if (value == _deltaX) return;
                _deltaX = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecoilGroup)]
        [DisplayName("Average Y Offset")]
        [SliderLimits(1, 10, 0, 1, 1)]
        [DefaultValue(3)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int DeltaY
        {
            get => _deltaY;
            set
            {
                if (value == _deltaY) return;
                _deltaY = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecoilGroup)]
        [DisplayName("Sensitivity adjustment")]
        [SliderLimits(0.1f, 5)]
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float SensitivityScale
        {
            get => _sensitivityScale;
            set
            {
                if (value.Equals(_sensitivityScale)) return;
                _sensitivityScale = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecognitionGroup)]
        [DisplayName("Brightness adjustment")]
        [SliderLimits(0.3f, 1.18f, 2, 0.01f, 0.01f)]
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float BrightnessScale
        {
            get => _brightnessScale;
            set
            {
                if (value.Equals(_brightnessScale)) return;
                _brightnessScale = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecoilGroup)]
        [DisplayName("Unique weapon logic")]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UseUniqueWeaponLogic
        {
            get => _useUniqueWeaponLogic;
            set
            {
                if (value == _useUniqueWeaponLogic) return;
                _useUniqueWeaponLogic = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool LockToGameWindow
        {
            get => _lockToGameWindow;
            set
            {
                if (value == _lockToGameWindow) return;
                _lockToGameWindow = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember, PropertyGroup(RecognitionGroup)]
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool FullScreenMode
        {
            get => _fullScreenMode;
            set
            {
                if (value == _fullScreenMode) return;
                _fullScreenMode = value;
                OnPropertyChanged();
            }
        }

        [PropertyMember]
        [DisplayName("Select the game:")]
        [DefaultValue("Apex Legends")]
        [PropertyOrder(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string SelectedGameName
        {
            get => _selectedGameName;
            set
            {
                if (value == _selectedGameName) return;
                _selectedGameName = value;
                OnPropertyChanged();
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
