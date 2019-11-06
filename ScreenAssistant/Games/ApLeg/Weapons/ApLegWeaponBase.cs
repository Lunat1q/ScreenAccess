using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiqSoft.ScreenAssistant.Annotations;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal abstract class ApLegWeaponBase : IWeapon
    {
        protected const double MinFireRate = 0.04;
        protected const double MaxFireRate = 0.05;
        protected static readonly double ShotsPerSecond = 1 / ((MaxFireRate + MinFireRate) / 2);
        protected static Random Rnd = new Random();
        protected readonly double ShotsPerBurst;
        private readonly string _recognizedName;
        private float _sensitivityScale;
        private ObservableCollection<WeaponModule> _installedModules;
        private bool _isActive;
        private DateTime _lastDetection;

        protected ApLegWeaponBase(string name, double burstSeconds, string recognizedName, int numberOfModules) // recognized name is for test purpose
        {
            this._recognizedName = recognizedName;
            this.NumberOfModules = numberOfModules;
            this.ShotsPerBurst = ShotsPerSecond * burstSeconds;
            this.Name = name;
            this.InitializeModules();
        }

        public string Name { get; protected set; }

        public double AdjustmentCoefficient { get; protected set; }

        public ObservableCollection<WeaponModule> InstalledModules
        {
            get => this._installedModules;
            private set
            {
                if (Equals(value, this._installedModules)) return;
                this._installedModules = value;
                this.OnPropertyChanged();
            }
        }

        public virtual void SetModule(int id, WeaponModuleType moduleType)
        {
            this.InstalledModules[id].Type = moduleType;
        }

        public bool IsTheSameWeapon(string weaponName)
        {
            return this.Name.Equals(weaponName, StringComparison.OrdinalIgnoreCase);
        }

        public abstract double AdjustMouse(int shotNumber);

        private void InitializeModules()
        {
            this.InstalledModules = new ObservableCollection<WeaponModule>();
            for (var i = 0; i < this.NumberOfModules; i++)
            {
                this.InstalledModules.Add(new WeaponModule());
            }
        }

        public WeaponModuleType GetModuleType(int idx)
        {
            return idx < this.InstalledModules.Count ? this.InstalledModules[idx].Type : WeaponModuleType.None;
        }

        public void SetSensitivityScale(float sensitivityScale)
        {
            this._sensitivityScale = sensitivityScale;
        }

        public abstract bool IsDefault();

        public bool IsActive
        {
            get => this._isActive;
            set
            {
                if (value == this._isActive) return;
                this._isActive = value;
                this.OnPropertyChanged();
            }
        }

        public int NumberOfModules { get; }

        public event MouseMovedEvent MouseMoved;

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
            var hOffset = this.AdjustmentCoefficient > 0.001 ? horizontalOffset : 0;
            var xDelta = (int)(hOffset * this._sensitivityScale);
            var yDelta = (int)(verticalOffset * this.AdjustmentCoefficient * this._sensitivityScale);
            MouseControl.Move(xDelta, yDelta);
            this.OnMouseMoved(xDelta, yDelta);
        }

        public override string ToString()
        {
            return$"{this.Name} : {this._recognizedName}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnMouseMoved(int xDelta, int yDelta)
        {
            var args = new MouseMovedEventArgs(xDelta, yDelta, -1);
            this.MouseMoved?.Invoke(this, args);
        }

        public bool PossiblyOutdated()
        {
            return this._lastDetection < DateTime.Now.AddSeconds(-30);
        }

        public void Refresh()
        {
            this._lastDetection = DateTime.Now;
        }
    }
}