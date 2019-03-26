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
            if (shotNumber < 25)
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 30);
                horizontalOffset = Rnd.NextDouble() * 1 + 1;
                verticalOffset = Rnd.NextDouble() * (8 - 6) + 6;
            }
            else
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 140);
                var hAdj = shotNumber > 50 ? -1.3d : 1;
                horizontalOffset = hAdj * (Rnd.NextDouble() * 0.5 + 1);
                verticalOffset = Rnd.NextDouble() * 0.5 + 2;
            }

            MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));

            return GetAdjustmentTime(1d);
        }
    }
}