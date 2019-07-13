using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GlobalHook;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Games;
using TiqSoft.ScreenAssistant.Properties;
using TiqUtils.TypeSpeccific;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal class MainLogicController : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private IWeaponFactory _weaponFactory;
        private bool _enabled;
        private int _deltaY;
        private int _deltaX;
        private bool _working;
        private bool _mouseDown;
        private CancellationTokenSource _mouseTaskCts;
        private CancellationTokenSource _mainTaskCts;
        private CancellationTokenSource _weaponRecognitionCts;
        private double _adjustmentCoefficient = 1;
        private ObservableCollection<IWeapon> _weapons;
        private bool _useWeaponLogic;
        private float _sensitivityScale;

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

        public string CurrentVersionInfo { get; }

        public MainLogicController(int deltaX, int deltaY, float sensitivityScale, bool useWeaponLogic, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            UseWeaponLogic = useWeaponLogic;
            DeltaX = deltaX;
            DeltaY = deltaY;
            SensitivityScale = sensitivityScale;
            HotKeysController = new BindingController();
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'K', Toggle);
            HotKeysController.Start(true);
            CurrentVersionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public MainLogicController() : this(2, 3, 1, false, null)
        {
        }

        public void SetGameFactory(IWeaponFactory factory)
        {
            if (Enabled || Working)
            {
                Stop();
            }
            _weaponFactory = factory;
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

        public int DeltaY
        {
            get => _deltaY;
            set
            {
                if (value == _deltaY) return;
                _deltaY = value;
                OnPropertyChanged();
                if (Weapons == null) return;
                foreach (var weapon in Weapons)
                {
                    weapon.SetOffsets(_deltaX, _deltaY);
                }
            }
        }

        public int DeltaX
        {
            get => _deltaX;
            set
            {
                if (value == _deltaX) return;
                _deltaX = value;
                OnPropertyChanged();
                if (Weapons == null) return;
                foreach (var weapon in Weapons)
                {
                    weapon.SetOffsets(_deltaX, _deltaY);
                }
            }
        }

        public bool UseWeaponLogic
        {
            get => _useWeaponLogic;
            set
            {
                if (value == _useWeaponLogic) return;
                _useWeaponLogic = value;
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
            get => _sensitivityScale;
            set
            {
                if (value.Equals(_sensitivityScale)) return;
                _sensitivityScale = value;
                OnPropertyChanged();
                if (Weapons == null) return;
                foreach (var weapon in Weapons)
                {
                    weapon.SetOffsets(_deltaX, _deltaY);
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
            _mouseTaskCts = new CancellationTokenSource();
            Task.Run(() => CheckForMouse(_mouseTaskCts.Token));
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
                newWeapon.SetOffsets(_deltaX, _deltaY);
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
                    if (UseWeaponLogic)
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
                                    currentWeapon.SetOffsets(_deltaX, _deltaY);
                                    _dispatcher.Invoke(() =>
                                        {
                                            return Weapons[i - 1] = newDetectedWeapon;
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

        private async Task StartActiveElementRecognition(CancellationToken token)
        {
            try
            {

                while (true)
                {
                    if (UseWeaponLogic)
                    {
                        var activeWeapon = _weaponFactory.Recognizer.GetActiveWeapon();
                        for (int i = 0; i < Weapons.Count; i++)
                        {
                            var weapon = Weapons[i];
                            weapon.IsActive = (i + 1) == activeWeapon;
                            if (weapon.IsActive)
                            {
                                _weaponFactory.WeaponPostProcess(weapon);
                            }
                        }
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
                    if (MouseDown)
                    {
                        var weapon = Weapons.First(x => x.IsActive);

                        var delay = weapon.AdjustMouse(simShots);
                        AdjustmentCoefficient = weapon.AdjustmentCoefficient;

                        await Task.Delay((int)(delay * 1000), token);
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
