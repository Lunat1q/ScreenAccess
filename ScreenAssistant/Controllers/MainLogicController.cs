using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GlobalHook;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Games.ApLeg;
using TiqSoft.ScreenAssistant.Properties;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqUtils.TypeSpeccific;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal class MainLogicController : INotifyPropertyChanged
    {
        private bool _enabled;
        private int _deltaY;
        private int _deltaX;
        private bool _working;
        private bool _mouseDown;
        private CancellationTokenSource _mouseTaskCts;
        private CancellationTokenSource _mainTaskCts;
        private CancellationTokenSource _weaponRecognitionCts;
        private double _adjustmentCoefficient = 1;
        private string _weapon2Name;
        private string _weapon1Name;
        private bool _firstWeaponActive;
        private IWeapon _weapon1;
        private IWeapon _weapon2;
        private bool _useWeaponLogic;

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

        public MainLogicController(int deltaX, int deltaY, bool useWeaponLogic)
        {
            UseWeaponLogic = useWeaponLogic;
            DeltaX = deltaX;
            DeltaY = deltaY;
            HotKeysController = new BindingController();
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'K', Toggle);
            HotKeysController.Start(true);
            ResetEquippedWeapons();
        }

        private void ResetEquippedWeapons()
        {
            Weapon1Name = "Unknown";
            Weapon2Name = "Unknown";
        }

        public MainLogicController() : this(2, 3, false)
        {
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
                _weapon1?.SetOffsets(_deltaX, _deltaY);
                _weapon2?.SetOffsets(_deltaX, _deltaY);
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
                _weapon1?.SetOffsets(_deltaX, _deltaY);
                _weapon2?.SetOffsets(_deltaX, _deltaY);
            }
        }

        public bool FirstWeaponActive
        {
            get => _firstWeaponActive;
            set
            {
                if (value == _firstWeaponActive) return;
                _firstWeaponActive = value;
                OnPropertyChanged();
            }
        }

        public string Weapon1Name
        {
            get => _weapon1Name;
            set
            {
                if (value == _weapon1Name) return;
                _weapon1Name = value;
                OnPropertyChanged();
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

        public string Weapon2Name
        {
            get => _weapon2Name;
            set
            {
                if (value == _weapon2Name) return;
                _weapon2Name = value;
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
            CreateDefaultWeapons();
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
            _weapon1 = ApLegFactory.ConstructDefault();
            _weapon2 = ApLegFactory.ConstructDefault();
            _weapon1.SetOffsets(_deltaX, _deltaY);
            _weapon2.SetOffsets(_deltaX, _deltaY);
            Weapon1Name = _weapon1.Name;
            Weapon2Name = _weapon2.Name;
        }

        private async Task StartWeaponRecognition(CancellationToken token)
        {
            try
            {

                while (true)
                {
                    if (UseWeaponLogic)
                    {
                        var weapon1RecognizedName = WeaponTypeScreenRecognizer.GetWeapon1FromScreen();
                        var weapon2RecognizedName = WeaponTypeScreenRecognizer.GetWeapon2FromScreen();
                        if (!weapon1RecognizedName.Empty())
                        {
                            var newDetectedWeapon = ApLegFactory.ConstructFromRecognizedString(
                                weapon1RecognizedName, 
                                _weapon1,
                                DeltaX, 
                                DeltaY
                            );
                            if (!newDetectedWeapon.IsDefault() && !_weapon1.Equals(newDetectedWeapon))
                            {
                                FirstWeaponActive = true;
                                _weapon1 = newDetectedWeapon;
                                _weapon1.SetOffsets(_deltaX, _deltaY);
                                Weapon1Name = _weapon1.Name;
                            }
                        }

                        if (!weapon2RecognizedName.Empty())
                        {
                            var newDetectedWeapon = ApLegFactory.ConstructFromRecognizedString(
                                weapon2RecognizedName,
                                _weapon2,
                                DeltaX,
                                DeltaY
                            );
                            if (!newDetectedWeapon.IsDefault() && !_weapon2.Equals(newDetectedWeapon))
                            {
                                FirstWeaponActive = false;
                                _weapon2 = newDetectedWeapon;
                                _weapon2.SetOffsets(_deltaX, _deltaY);
                                Weapon2Name = _weapon2.Name;
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
                ResetEquippedWeapons();
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
                        FirstWeaponActive = WeaponTypeScreenRecognizer.IsFirstWeaponActive();
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
                FirstWeaponActive = true;
            }
        }

        private IWeapon GetCurrentWeapon()
        {
            return _firstWeaponActive ? _weapon1 : _weapon2;
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
                        var weapon = GetCurrentWeapon();

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
