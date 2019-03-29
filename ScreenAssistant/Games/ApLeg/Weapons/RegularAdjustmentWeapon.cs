using System;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class RegularAdjustmentWeapon : ApLegWeaponBase
    {
        public RegularAdjustmentWeapon(string name, double burstSeconds, string recognizedName, WeaponAL type) 
            : base(name, burstSeconds, recognizedName, type)
        {
            if (Name == String.Empty)
            {
                Name = "Default";
            }
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, ShotsPerBurst);
            var horizontalOffset = Rnd.NextDouble() * DeltaX * 2 - DeltaX;
            var verticalOffset = Rnd.NextDouble() * (MaxOffsetY - MinOffsetY) + MinOffsetY;
            MoveMouse(horizontalOffset, verticalOffset);
            return Rnd.NextDouble() * (MaxFireRate - MinFireRate) + MinFireRate;
        }

        public override bool IsDefault()
        {
            return WeaponType == WeaponAL.Unknown;
        }
    }
}