using System;
using TiqUtils.TypeSpecific;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class RegularAdjustmentWeapon : ApLegWeaponBase
    {
        private const int DeltaX = 2;
        private const double DeltaYMin = 1.5;
        private const double DeltaYMax = 4.5;
        private readonly bool _isDefault = false;

        public RegularAdjustmentWeapon(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
            if (this.Name.Empty())
            {
                this._isDefault = true;
                this.Name = "Default";
            }
            else
            {
                this.Name += " [Reg]";
            }
        }

        public override double AdjustMouse(int shotNumber)
        {
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, this.ShotsPerBurst);
            var horizontalOffset = Rnd.NextDouble() * DeltaX * 2 - DeltaX;
            var verticalOffset = Rnd.NextDouble() * (DeltaYMax - DeltaYMin) + DeltaYMin;
            this.MoveMouse(horizontalOffset, verticalOffset);
            return Rnd.NextDouble() * (MaxFireRate - MinFireRate) + MinFireRate;
        }

        public override bool IsDefault()
        {
            return this._isDefault;
        }
    }
}