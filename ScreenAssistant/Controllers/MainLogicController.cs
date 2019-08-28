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
        private readonly LogicSettings _logicSettings;
        private ImageTestController _testController;
        private bool _mouseCentered;

        #region Properties
        private BindingController HotKeysController { get; }

        public bool MouseDown
        {
            get => _mouseDown;
            set
            {
                if (value == _mouseDown) return;
                _mouseDown = value;
                OnPropertyChanged();
            }
        }

        public bool MouseCentered
        {
            get => _mouseCentered;
            set
            {
#if DEBUG
                if (value == _mouseCentered) return;
                _mouseCentered = value;
                OnPropertyChanged();
#else
                _mouseCentered = value;
#endif
            }
        } // no need for notification for now.

        public string CurrentVersionInfo { get; }

        public MainLogicController(LogicSettings logicSettings, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _logicSettings = logicSettings;
            HotKeysController = new BindingController();
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'K', Toggle);
            HotKeysController.Start(true);
            CurrentVersionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TestController = ImageTestController.Instance;
        }

        public MainLogicController() : this(new LogicSettings(), null)
        {
        }

        public void SetGameFactory(IWeaponFactory factory)
        {
            if (Enabled || Working)
            {
                Stop();
            }
            _weaponFactory = factory;
            TestController.WeaponFactory = _weaponFactory;
            _weaponFactory.Recognizer.FullScreenMode = this.FullScreenMode;
            CreateDefaultWeapons();
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled) return;
                _enabled = value;
                OnPropertyChanged();
            }
        }

        public bool Working
        {
            get => _working;
            set
            {
                if (value == _working) return;
                _working = value;
                OnPropertyChanged();
            }
        }

        public ImageTestController TestController
        {
            get => _testController;
            set {
                _testController = value;
                OnPropertyChanged();
            }
        }

        public int DeltaY
        {
            get => _logicSettings.DeltaY;
            set
            {
                if (value == _logicSettings.DeltaY) return;
                _logicSettings.DeltaY = value;
                OnPropertyChanged();
                if (Weapons == null) return;
                foreach (var weapon in Weapons)
                {
                    weapon.SetOffsets(_logicSettings.DeltaX, _logicSettings.DeltaY);
                }
            }
        }

        public int DeltaX
        {
            get => _logicSettings.DeltaX;
            set
            {
                if (value == _logicSettings.DeltaX) return;
                _logicSettings.DeltaX = value;
                OnPropertyChanged();
                if (Weapons == null) return;
                foreach (var weapon in Weapons)
                {
                    weapon.SetOffsets(_logicSettings.DeltaX, _logicSettings.DeltaY);
                }
            }
        }

        public bool UseWeaponLogic
        {
            get => _logicSettings.UseWeaponLogic;
            set
            {
                if (value == _logicSettings.UseWeaponLogic) return;
                _logicSettings.UseWeaponLogic = value;
                OnPropertyChanged();
            }
        }

        public bool LockToGameWindow
        {
            get => _logicSettings.LockToGameWindow;
            set
            {
                if (value == _logicSettings.LockToGameWindow) return;
                _logicSettings.LockToGameWindow = value;
                OnPropertyChanged();
            }
        }

        public bool FullScreenMode
        {
            get => _logicSettings.FullScreenMode;
            set
            {
                if (value == _logicSettings.FullScreenMode) return;
                _logicSettings.FullScreenMode = value;
                _weaponFactory.Recognizer.FullScreenMode = value;
                OnPropertyChanged();
            }
        }

        public double AdjustmentCoefficient
        {
            get => _adjustmentCoefficient;
            set
            {
                if (value.Equals(_adjustmentCoefficient)) return;
                _adjustmentCoefficient = value;
                OnPropertyChanged();
            }
        }

        public float SensitivityScale
        {
            get => _logicSettings.SensitivityScale;
            set
            {
                if (value.Equals(_logicSettings.SensitivityScale)) return;
                _logicSettings.SensitivityScale = value;
                OnPropertyChanged();
                if (Weapons == null) return;
                foreach (var weapon in Weapons)
                {
                    weapon.SetSensitivityScale(_logicSettings.SensitivityScale);
                }
            }
        }

        public ObservableCollection<IWeapon> Weapons
        {
            get => _weapons;
            set
            {
                _weapons = value;
                OnPropertyChanged();
            }
        }

#endregion

#region INotify
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endregion

        private void Toggle()
        {
            if (Enabled)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        public void Start()
        {
            Enabled = true;
            _weaponFactory.Recognizer.FullScreenMode = FullScreenMode;
            _mouseTaskCts = new CancellationTokenSource();
            Task.Run(() => CheckForMouse(_mouseTaskCts.Token));
            Task.Run(() => CheckForMousePos(_mouseTaskCts.Token));
            _mainTaskCts = new CancellationTokenSource();
            Task.Run(() => StartProcessingInput(_mainTaskCts.Token));
            _weaponRecognitionCts = new CancellationTokenSource();
            Task.Run(() => StartWeaponRecognition(_weaponRecognitionCts.Token));
            Task.Run(() => StartActiveElementRecognition(_weaponRecognitionCts.Token));
        }

        private void CreateDefaultWeapons()
        {
            Weapons = new ObservableCollection<IWeapon>();
            for (var i = 0; i < _weaponFactory.NumberOfWeapons; i++)
            {
                var newWeapon = _weaponFactory.Default();
                newWeapon.SetOffsets(_logicSettings.DeltaX, _logicSettings.DeltaY);
                Weapons.Add(newWeapon);
            }
        }

        private async Task StartWeaponRecognition(CancellationToken token)
        {
            try
            {
                CreateDefaultWeapons();
                while (true)
                {
                    if (UseWeaponLogic && CheckWindowLock())
                    {
                        for (var i = 1; i <= _weaponFactory.NumberOfWeapons; i++)
                        {
                            var weaponRecognizedName = _weaponFactory.Recognizer.GetWeaponFromScreen(i);
                            if (!weaponRecognizedName.Empty())
                            {
                                var currentWeapon = Weapons[i - 1];
                                var newDetectedWeapon = _weaponFactory.FromRecognizedString(
                                    weaponRecognizedName,
                                    currentWeapon,
                                    DeltaX,
                                    DeltaY,
                                    SensitivityScale
                                );

                                if (!newDetectedWeapon.IsDefault() && !currentWeapon.Equals(newDetectedWeapon))
                                {
                                    currentWeapon.IsActive = true;
                                    currentWeapon.SetOffsets(_logicSettings.DeltaX, _logicSettings.DeltaY);
                                    var i1 = i;
                                    _dispatcher.Invoke(() =>
                                        {
                                            return Weapons[i1 - 1] = newDetectedWeapon;
                                        }
                                    );
                                }
                            }
                        }
                    }

                    await Task.Delay(3000, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                CreateDefaultWeapons();
            }
        }

        private bool CheckWindowLock()
        {
            return !_logicSettings.LockToGameWindow ||
                   _weaponFactory.LockedToApplication.Empty() ||
                   _weaponFactory.LockedToApplication.Equals(WinApiHelper.GetActiveProcessName())
                ;
        }

        private async Task StartActiveElementRecognition(CancellationToken token)
        {
            var currentActiveWeapon = 0;
            try
            {
                while (true)
                {
                    if (UseWeaponLogic && CheckWindowLock())
                    {
                        var activeWeapon = _weaponFactory.Recognizer.GetActiveWeapon();
                        
                        for (var i = 0; i < Weapons.Count; i++)
                        {
                            var weapon = Weapons[i];
                            weapon.IsActive = (i + 1) == activeWeapon;
                            if (weapon.IsActive && currentActiveWeapon == activeWeapon)
                            {
                                _weaponFactory.WeaponPostProcess(weapon);
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
                Weapons[0].IsActive = true;
            }
        }
        
        private async Task StartProcessingInput(CancellationToken token)
        {
            try
            {
                
                Working = true;
                var simShots = 0;
                while (Working)
                {
                    if (MouseDown && MouseCentered && CheckWindowLock())
                    {
                        var weapon = Weapons.FirstOrDefault(x => x.IsActive);
                        if (weapon != null)
                        {
                            var delay = weapon.AdjustMouse(simShots);
                            AdjustmentCoefficient = weapon.AdjustmentCoefficient;
                            await Task.Delay((int)(delay * 1000), token);
                        }
                        simShots++;
                    }
                    else
                    {
                        AdjustmentCoefficient = 1;
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
                Working = false;
            }
        }

        private async Task CheckForMouse(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    MouseDown = KeyStatesHook.IsLeftMouseDown();
                    await Task.Delay(10, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                MouseDown = false;
            }
        }



        private async Task CheckForMousePos(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    MouseCentered = WinApiHelper.IsCursorAtTheCenter();
                    await Task.Delay(340, token);
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
            finally
            {
                MouseDown = false;
            }
        }

        public void Stop()
        {
            Enabled = false;
            Working = false;
            _mouseTaskCts.Cancel();
            _weaponRecognitionCts.Cancel();
            _mainTaskCts.Cancel();
        }
    }
}
