using System;
using TiqUtils.TypeSpeccific;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class RegularAdjustmentWeapon : ApLegWeaponBase
    {
        private readonly bool _isDefault = false;

        public RegularAdjustmentWeapon(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
            if (Name.Empty())
            {
                _isDefault = true;
                Name = "Default";
            }
            else
            {
                Name += " [Reg]";
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
            return _isDefault;
        }
    }
}