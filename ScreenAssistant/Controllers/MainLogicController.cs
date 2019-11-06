using GlobalHook;
using GlobalHook.Event;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Core.Settings;
using TiqSoft.ScreenAssistant.Games;
using TiqSoft.ScreenAssistant.Properties;
using TiqUtils.TypeSpecific;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal class MainLogicController : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private IWeaponFactory _weaponFactory;
        private bool _enabled;
        private bool _working;
        private bool _mouseDown;
        private CancellationTokenSource _mouseTaskCts;
        private CancellationTokenSource _mainTaskCts;
        private CancellationTokenSource _weaponRecognitionCts;
        private double _adjustmentCoefficient = 1;
        private ObservableCollection<IWeapon> _weapons;
        private readonly ScreenAssistantSettings _settings;
        private ImageTestController _testController;
        private bool _mouseCentered;
        private PatternTestController _patternController;

        #region Properties
        private BindingController HotKeysController { get; }

        public bool MouseDown
        {
            get => this._mouseDown;
            set
            {
                if (value == this._mouseDown) return;
                this._mouseDown = value;
                this.OnPropertyChanged();
            }
        }

        public bool MouseCentered
        {
            get => this._mouseCentered;
            set
            {
#if DEBUG
                if (value == this._mouseCentered) return;
                this._mouseCentered = value;
                this.OnPropertyChanged();
#else
                _mouseCentered = value;
#endif
            }
        } // no need for notification for now.

        public string CurrentVersionInfo { get; }

        public MainLogicController(ScreenAssistantSettings logicSettings, Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
            this._settings = logicSettings;
            this.HotKeysController = new BindingController();
            this.HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'K', this.Toggle);
            this.HotKeysController.Start(true);
            this.CurrentVersionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.TestController = ImageTestController.Instance;
            this.PatternController = PatternTestController.Instance;
            this.PatternController.Dispatcher = dispatcher;
            this.SetGameFactory(GamesHelper.GetFactoryByGameName(logicSettings.SelectedGameName));
            logicSettings.PropertyChanged += this.LogicSettings_PropertyChanged;
        }

        private void LogicSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var settings = (ScreenAssistantSettings)sender;
            if (e.PropertyName.Equals(nameof(this._settings.SelectedGameName)))
            {
                this.SetGameFactory(GamesHelper.GetFactoryByGameName(settings.SelectedGameName));
            }
            else if (e.PropertyName.Equals(nameof(this._settings.FullScreenMode)) && this._weaponFactory?.Recognizer != null)
            {
                this._weaponFactory.Recognizer.FullScreenMode = this._settings.FullScreenMode;
            }
            else if (e.PropertyName.Equals(nameof(this._settings.BrightnessScale)))
            {
                this._weaponFactory?.Recognizer?.SetBrightness(this._settings.BrightnessScale);
            }
            else if (e.PropertyName.Equals(nameof(this._settings.SensitivityScale)))
            {
                if (this.Weapons == null) return;
                foreach (var weapon in this.Weapons)
                {
                    weapon.SetSensitivityScale(this._settings.SensitivityScale);
                }
            }
        }

        public MainLogicController() : this(new ScreenAssistantSettings(), null)
        {
        }

        public void SetGameFactory(IWeaponFactory factory)
        {
            if (this.Enabled || this.Working)
            {
                this.Stop();
            }

            this._weaponFactory = factory;
            this.TestController.WeaponFactory = this._weaponFactory;
            this.PatternController.WeaponFactory = this._weaponFactory;
            this._weaponFactory.Recognizer.FullScreenMode = this._settings.FullScreenMode;
            this._weaponFactory.Recognizer.SetBrightness(this._settings.BrightnessScale);
            this.CreateDefaultWeapons();
        }
        
        public bool Enabled
        {
            get => this._enabled;
            set
            {
                if (value == this._enabled) return;
                this._enabled = value;
                this.OnPropertyChanged();
            }
        }

        public bool Working
        {
            get => this._working;
            set
            {
                if (value == this._working) return;
                this._working = value;
                this.OnPropertyChanged();
            }
        }

        public ImageTestController TestController
        {
            get => this._testController;
            set {
                this._testController = value;
                this.OnPropertyChanged();
            }
        }

        public PatternTestController PatternController
        {
            get => this._patternController;
            set
            {
                this._patternController = value;
                this.OnPropertyChanged();
            }
        }

        public double AdjustmentCoefficient
        {
            get => this._adjustmentCoefficient;
            set
            {
                if (value.Equals(this._adjustmentCoefficient)) return;
                this._adjustmentCoefficient = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<IWeapon> Weapons
        {
            get => this._weapons;
            set
            {
                this._weapons = value;
                this.OnPropertyChanged();
            }
        }

#endregion

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Toggle()
        {
            if (this.Enabled)
            {
                this.Stop();
            }
            else
            {
                this.Start();
            }
        }

        public void Start()
        {
            this.Enabled = true;
            this._weaponFactory.Recognizer.FullScreenMode = this._settings.FullScreenMode;
            this._mouseTaskCts = new CancellationTokenSource();
            Task.Run(() => this.CheckForMouse(this._mouseTaskCts.Token));
            Task.Run(() => this.CheckForMousePos(this._mouseTaskCts.Token));
            this._mainTaskCts = new CancellationTokenSource();
            Task.Run(() => this.StartProcessingInput(this._mainTaskCts.Token));
            this._weaponRecognitionCts = new CancellationTokenSource();
            Task.Run(() => this.StartWeaponRecognition(this._weaponRecognitionCts.Token));
            Task.Run(() => this.StartActiveElementRecognition(this._weaponRecognitionCts.Token));
        }

        private void CreateDefaultWeapons()
        {
            this.Weapons = new ObservableCollection<IWeapon>();
            for (var i = 0; i < this._weaponFactory.NumberOfWeapons; i++)
            {
                var newWeapon = this._weaponFactory.Default();
                this.Weapons.Add(newWeapon);
            }
        }

        private async Task StartWeaponRecognition(CancellationToken token)
        {
            try
            {
                this.CreateDefaultWeapons();
                while (true)
                {
                    if (this.CheckWindowLock())
                    {
                        var weaponFound = false;
                        for (var i = 1; i <= this._weaponFactory.NumberOfWeapons; i++)
                        {
                            var weaponRecognizedName = await this._weaponFactory.Recognizer.GetWeaponFromScreen(i);
                            var currentWeapon = this.Weapons[i - 1];
                            if (!weaponRecognizedName.Empty())
                            {
                                this.UpdateWeaponFromRecognizedName(weaponRecognizedName, currentWeapon, i);
                                weaponFound = true;
                            }
                            else if (currentWeapon.IsActive && !currentWeapon.IsDefault() && currentWeapon.PossiblyOutdated()) //attempt to auto-reset a weapon
                            {
                                this.SetWeapon(i, this._weaponFactory.Default());
                            }
                        }

                        this.RefreshWeapons(weaponFound);
                    }

                    await Task.Delay(500, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                this.CreateDefaultWeapons();
            }
        }

        private void UpdateWeaponFromRecognizedName(string weaponRecognizedName, IWeapon currentWeapon, int i)
        {
            var newDetectedWeapon = this._weaponFactory.FromRecognizedString(
                weaponRecognizedName,
                currentWeapon,
                this._settings.SensitivityScale
            );

            if (!newDetectedWeapon.IsDefault() && !currentWeapon.Equals(newDetectedWeapon))
            {
                currentWeapon.IsActive = true;
                this.SetWeapon(i, newDetectedWeapon);
            }
        }

        private void RefreshWeapons(bool weaponFound)
        {
            if (weaponFound)
            {
                foreach (var weapon in this.Weapons)
                {
                    weapon.Refresh();
                }
            }
        }

        private void SetWeapon(int i1, IWeapon newDetectedWeapon)
        {
            this._dispatcher.Invoke(() => { return this.Weapons[i1 - 1] = newDetectedWeapon; });
        }

        private bool CheckWindowLock()
        {
            return !this._settings.LockToGameWindow || this._weaponFactory.LockedToApplication.Empty() || this._weaponFactory.LockedToApplication.Equals(WinApiHelper.GetActiveProcessName())
                ;
        }

        private async Task StartActiveElementRecognition(CancellationToken token)
        {
            var currentActiveWeapon = 0;
            try
            {
                while (true)
                {
                    if (this.CheckWindowLock())
                    {
                        var activeWeapon = this._weaponFactory.Recognizer.GetActiveWeapon();
                        
                        for (var i = 0; i < this.Weapons.Count; i++)
                        {
                            var weapon = this.Weapons[i];
                            weapon.IsActive = (i + 1) == activeWeapon;
                            if (weapon.IsActive && currentActiveWeapon == activeWeapon)
                            {
                                this._weaponFactory.WeaponPostProcess(weapon);
                            }
                        }

                        currentActiveWeapon = activeWeapon;
                    }

                    await Task.Delay(500, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                this.Weapons[0].IsActive = true;
            }
        }
        
        private async Task StartProcessingInput(CancellationToken token)
        {
            try
            {
                this.Working = true;
                var simShots = 0;
                while (this.Working)
                {
                    if (this.MouseDown && this.MouseCentered && this.CheckWindowLock())
                    {
                        var weapon = this.Weapons.FirstOrDefault(x => x.IsActive);
                        if (weapon != null)
                        {
                            var delay = weapon.AdjustMouse(simShots);
                            this.AdjustmentCoefficient = weapon.AdjustmentCoefficient;
                            await Task.Delay((int)(delay * 1000), token);
                        }
                        simShots++;
                    }
                    else
                    {
                        this.AdjustmentCoefficient = 1;
                        simShots = 0;
                    }
                    await Task.Delay(16, token);
                }

            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                this.Working = false;
            }
        }

        private async Task CheckForMouse(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    this.MouseDown = KeyStatesHook.IsLeftMouseDown();
                    await Task.Delay(10, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                this.MouseDown = false;
            }
        }



        private async Task CheckForMousePos(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    this.MouseCentered = WinApiHelper.IsCursorAtTheCenter();
                    await Task.Delay(340, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                this.MouseDown = false;
            }
        }

        public void Stop()
        {
            this.Enabled = false;
            this.Working = false;
            this._mouseTaskCts.Cancel();
            this._weaponRecognitionCts.Cancel();
            this._mainTaskCts.Cancel();
        }
    }
}
