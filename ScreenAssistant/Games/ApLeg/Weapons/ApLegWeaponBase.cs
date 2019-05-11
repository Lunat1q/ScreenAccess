using System;
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

        protected ApLegWeaponBase(string name, double burstSeconds, string recognizedName, WeaponAL weaponType) // recognized name is for test purpose
        {
            _recognizedName = recognizedName;
            WeaponType = weaponType;
            ShotsPerBurst = ShotsPerSecond * burstSeconds;
            Name = name;
        }

        public string Name { get; protected set; }

        public int DeltaX { get; private set; }

        public double AdjustmentCoefficient { get; protected set; }

        public WeaponAL WeaponType { get; }

        public bool IsTheSameWeapon(string weaponName)
        {
            return Name.Equals(weaponName, StringComparison.OrdinalIgnoreCase);
        }

        public abstract double AdjustMouse(int shotNumber);

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
    }
}