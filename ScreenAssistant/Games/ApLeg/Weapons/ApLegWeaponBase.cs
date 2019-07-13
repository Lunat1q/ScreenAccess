using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiqSoft.ScreenAssistant.Annotations;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal abstract class ApLegWeaponBase : IWeapon
    {
        protected const double MinFireRate = 0.04;
        protected const double MaxFireRate = 0.05;
        protected static readonly double ShotsPerSecond = 1 / ((MaxFireRate + MinFireRate) / 2);
        protected static Random Rnd = new Random();
        protected double MinOffsetY;
        protected double MaxOffsetY;
        protected readonly double ShotsPerBurst;
        private readonly string _recognizedName;
        private float _sensitivityScale;
        private ObservableCollection<WeaponModule> _installedModules;
        private bool _isActive;

        protected ApLegWeaponBase(string name, double burstSeconds, string recognizedName, int numberOfModules) // recognized name is for test purpose
        {
            _recognizedName = recognizedName;
            NumberOfModules = numberOfModules;
            ShotsPerBurst = ShotsPerSecond * burstSeconds;
            Name = name;
            InitializeModules();
        }

        public string Name { get; protected set; }

        public int DeltaX { get; private set; }

        public double AdjustmentCoefficient { get; protected set; }

        public ObservableCollection<WeaponModule> InstalledModules
        {
            get => _installedModules;
            private set
            {
                if (Equals(value, _installedModules)) return;
                _installedModules = value;
                OnPropertyChanged();
            }
        }

        public bool IsTheSameWeapon(string weaponName)
        {
            return Name.Equals(weaponName, StringComparison.OrdinalIgnoreCase);
        }

        public abstract double AdjustMouse(int shotNumber);

        private void InitializeModules()
        {
            InstalledModules = new ObservableCollection<WeaponModule>();
            for (var i = 0; i < NumberOfModules; i++)
            {
                InstalledModules.Add(new WeaponModule());
            }
        }

        public void SetOffsets(int deltaX, int deltaY)
        {
            MinOffsetY = deltaX * 0.5;
            MaxOffsetY = deltaY * 1.5;
            DeltaX = deltaX;
        }

        public void SetSensitivityScale(float sensitivityScale)
        {
            _sensitivityScale = sensitivityScale;
        }

        public abstract bool IsDefault();

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value == _isActive) return;
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfModules { get; }

        protected static double CalculateAdjustment(int shotNumber, double shotsPerBurst)
        {
            if (shotNumber > shotsPerBurst)
            {
                shotsPerBurst = shotNumber;
            }
            var result = (1 - shotNumber / shotsPerBurst) * 2;
            return result > 1 ? 1 : result;
        }

        protected void MoveMouse(double horizontalOffset, double verticalOffset)
        {
            int hOffset = AdjustmentCoefficient > 0.001 ? (int)horizontalOffset : 0;
            MouseControl.Move((int)(hOffset * _sensitivityScale), (int)(verticalOffset * AdjustmentCoefficient * _sensitivityScale));
        }

        public override string ToString()
        {
            return$"{Name} : {_recognizedName}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}