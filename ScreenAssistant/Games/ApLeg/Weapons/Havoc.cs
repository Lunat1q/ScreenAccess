using System;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Havoc : UniqueLogicWeapon
    {
        public Havoc(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.Havoc)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            if (shotNumber > 6)
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 40);
                var horizontalOffset = Rnd.NextDouble() * 1 * 2 - 1;
                var verticalOffset = Rnd.NextDouble() + 5.5d;
                MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
            }

            return GetAdjustmentTime(1d);
        }
    }
}