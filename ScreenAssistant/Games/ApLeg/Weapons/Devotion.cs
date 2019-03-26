using System;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Devotion : UniqueLogicWeapon
    {
        public Devotion(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.Devotion)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            double horizontalOffset;
            double verticalOffset;
            if (shotNumber < 15)
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 20);
                horizontalOffset = Rnd.NextDouble() * 1 + 1;
                verticalOffset = Rnd.NextDouble() * (2) + 5;
            }
            else
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 80);
                var hAdj = shotNumber > 25 ? -1.5d : 1.5d; 
                horizontalOffset = hAdj * (Rnd.NextDouble() * 0.5 + 1);
                verticalOffset = Rnd.NextDouble() * 0.5 + 2d;
            }

            MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));

            return GetAdjustmentTime(1d);
        }
    }
}