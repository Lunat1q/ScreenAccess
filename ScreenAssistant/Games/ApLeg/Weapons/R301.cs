using System;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class R301 : UniqueLogicWeapon
    {
        public R301(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.R301)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 15);
            var hAdj = shotNumber > 10 ? 3 : -1;
            var horizontalOffset = hAdj * (Rnd.NextDouble() * 1 + 1);
            if (AdjustmentCoefficient < 0.001)
            {
                horizontalOffset = 0;
            }
            var verticalOffset = Rnd.NextDouble() + 4d;
            MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));

            return GetAdjustmentTime(1d);
        }
    }
}