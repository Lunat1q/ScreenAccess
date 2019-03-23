using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GlobalHook;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Helpers;
using TiqSoft.ScreenAssistant.Properties;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal class MainLogicController : INotifyPropertyChanged
    {
        private const double MinFireRate = 0.04;
        private const double MaxFireRate = 0.05;
        readonly double _shotsPerSecond = 1 / ((MaxFireRate + MinFireRate) / 2);
        readonly int _burstSeconds = 2;
        private readonly Random _rnd;
        private bool _enabled;
        private int _deltaY;
        private int _deltaX;
        private bool _working;
        private bool _mouseDown;
        private CancellationTokenSource _mouseTaskCts;
        private CancellationTokenSource _mainTaskCts;
        private CancellationTokenSource _weaponRecognitionCts;
        private double _adjustmentCoefficient = 1;
        private string _weapon2;
        private string _weapon1;
        private bool _firstWeaponActive;
        private WeaponAL _weapon1Type;
        private WeaponAL _weapon2Type;
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
            _rnd = new Random();
        }

        private void ResetEquippedWeapons()
        {
            Weapon1 = "Unknown";
            Weapon2 = "Unknown";
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

        public string Weapon1
        {
            get => _weapon1;
            set
            {
                if (value == _weapon1) return;
                _weapon1 = value;
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

        public string Weapon2
        {
            get => _weapon2;
            set
            {
                if (value == _weapon2) return;
                _weapon2 = value;
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
            _mouseTaskCts = new CancellationTokenSource();
            Task.Run(() => CheckForMouse(_mouseTaskCts.Token));
            _mainTaskCts = new CancellationTokenSource();
            Task.Run(() => StartProcessingInput(_mainTaskCts.Token));
            _weaponRecognitionCts = new CancellationTokenSource();
            Task.Run(() => StartWeaponRecognition(_weaponRecognitionCts.Token));
            Task.Run(() => StartActiveElementRecognition(_weaponRecognitionCts.Token));
        }

        private async Task StartWeaponRecognition(CancellationToken token)
        {
            try
            {

                while (true)
                {
                    if (UseWeaponLogic)
                    {
                        var weapon1 = WeaponTypeScreenRecognizer.GetWeapon1FromScreen();
                        var weapon2 = WeaponTypeScreenRecognizer.GetWeapon2FromScreen();
                        if (weapon1 != WeaponAL.Unknown)
                        {
                            FirstWeaponActive = true;
                            Weapon1 = weapon1.GetWeaponName();
                            _weapon1Type = weapon1;
                        }

                        if (weapon2 != WeaponAL.Unknown)
                        {
                            FirstWeaponActive = false;
                            Weapon2 = weapon2.GetWeaponName();
                            _weapon2Type = weapon2;
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

        private WeaponAL GetCurrentWeapon()
        {
            return _firstWeaponActive ? _weapon1Type : _weapon2Type;
        }

        private static bool IsNonAdjustableWeapon(WeaponAL weapon)
        {
            const WeaponAL nonAdjustable = WeaponAL.Wingman | WeaponAL.KRABER | WeaponAL.Peacekeeper |
                                           WeaponAL.MASTIFF | WeaponAL.Longbow | WeaponAL.TripleTake;
            return nonAdjustable.HasFlag(weapon);
        }
        
        private async Task StartProcessingInput(CancellationToken token)
        {
            try
            {
                var minOffsetY = _deltaY * 0.5;
                var maxOffsetY = _deltaY * 1.5;
                Working = true;
                var simShots = 0;
                var shotsPerBurst = _burstSeconds * _shotsPerSecond;
                while (Working)
                {
                    if (MouseDown)
                    {
                        var weapon = GetCurrentWeapon();
                        var timeOffset = 1d;

                        if (UseWeaponLogic && IsNonAdjustableWeapon(weapon))
                        {
                            AdjustmentCoefficient = 0;
                        }
                        else if (UseWeaponLogic && ShouldAdjustSpecial(weapon))
                        {
                            timeOffset = AdjustSpecial(weapon, simShots++);
                        }
                        else
                        {
                            AdjustmentCoefficient = CalculateAdjustment(simShots, shotsPerBurst);
                            var horizontalOffset = _rnd.NextDouble() * _deltaX * 2 - _deltaX;
                            var verticalOffset = _rnd.NextDouble() * (maxOffsetY - minOffsetY) + minOffsetY;
                            MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
                            timeOffset = _rnd.NextDouble() * (MaxFireRate - MinFireRate) + MinFireRate;
                        }

                        await Task.Delay((int)(timeOffset * 1000), token);
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

        private double AdjustSpecial(WeaponAL weapon, int i)
        {
            double adjustTime = 1d;

            switch (weapon)
            {
                case WeaponAL.Devotion:
                {
                    double horizontalOffset;
                    double verticalOffset;
                    if (i < 25)
                    {
                        AdjustmentCoefficient = CalculateAdjustment(i, 30);
                        horizontalOffset = _rnd.NextDouble() * 1 + 1;
                        verticalOffset = _rnd.NextDouble() * (8 - 6) + 6;
                    }
                    else
                    {
                        AdjustmentCoefficient = CalculateAdjustment(i, 140);
                        var hAdj = i > 50 ? -1.3d : 1;
                        horizontalOffset = hAdj * (_rnd.NextDouble() * 0.5 + 1);
                        verticalOffset = _rnd.NextDouble() * 0.5 + 2;
                    }

                    MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.Havoc:
                {
                    if (i > 9)
                    {
                        AdjustmentCoefficient = CalculateAdjustment(i, 75);
                        var horizontalOffset = _rnd.NextDouble() * 1 * 2 - 1;
                        var verticalOffset = _rnd.NextDouble() + 5.5d;
                        MouseControl.Move((int) horizontalOffset, (int) (verticalOffset * AdjustmentCoefficient));
                    }

                    break;
                }
                case WeaponAL.R99:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 35);
                    var horizontalOffset = _rnd.NextDouble() * -1 - 1;
                    var verticalOffset = _rnd.NextDouble() + 7;
                    MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.Alternator:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 45);
                    var horizontalOffset = _rnd.NextDouble() * 1 * 2 - 1;
                    var verticalOffset = _rnd.NextDouble() + 5.5d;
                    MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.R301:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 30);
                    var hAdj = i > 15 ? 1 : -1;
                    var horizontalOffset = hAdj * (_rnd.NextDouble() * 1 + 1);
                    var verticalOffset = _rnd.NextDouble() + 3.5d;
                    MouseControl.Move((int)(horizontalOffset), (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.RE45:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 25);
                    var horizontalOffset = -1 * (_rnd.NextDouble() * 1 + 2d);
                    var verticalOffset = _rnd.NextDouble() + 5.5d;
                    MouseControl.Move((int)(horizontalOffset), (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.Flatline:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 40);
                    var hAdj = i > 20 && i < 25 ? -1d : 1.25;
                    var horizontalOffset = hAdj * (_rnd.NextDouble() * 1 + 1d);
                    var verticalOffset = _rnd.NextDouble() + 4d;
                    MouseControl.Move((int)(horizontalOffset), (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.Hemlok:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 5);
                    var horizontalOffset = 0;
                    var verticalOffset = _rnd.NextDouble() + 7d;
                    MouseControl.Move(horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
                    break;
                }
                case WeaponAL.Prowler:
                {
                    AdjustmentCoefficient = CalculateAdjustment(i, 14);
                    var horizontalOffset = 0;
                    var verticalOffset = _rnd.NextDouble() * 1 + 5.5d;
                    MouseControl.Move(horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
                    adjustTime = 0.3d;
                    break;
                }
            }

            return adjustTime * (_rnd.NextDouble() * (MaxFireRate - MinFireRate) + MinFireRate);
        }

        private static bool ShouldAdjustSpecial(WeaponAL weapon)
        {
            const WeaponAL nonAdjustable = WeaponAL.Devotion | WeaponAL.Havoc | WeaponAL.R99 |
                                           WeaponAL.Alternator | WeaponAL.R301 | WeaponAL.RE45 |
                                           WeaponAL.Flatline | WeaponAL.Hemlok | WeaponAL.Prowler;
            return nonAdjustable.HasFlag(weapon);
        }

        private static double CalculateAdjustment(int shotNumber, double shotsPerBurst)
        {
            if (shotNumber > shotsPerBurst)
            {
                shotsPerBurst = shotNumber;
            }
            var result = (1 - shotNumber / shotsPerBurst) * 2;
            return result > 1 ? 1 : result;
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
