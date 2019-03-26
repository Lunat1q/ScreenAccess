using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    // ReSharper disable once IdentifierTypo
    internal sealed class Flatline : UniqueLogicWeapon
    {
        // ReSharper disable once IdentifierTypo
        public Flatline(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.Flatline)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 40);
            var hAdj = shotNumber > 20 && shotNumber < 25 ? -1d : 1.25;
            var horizontalOffset = hAdj * (Rnd.NextDouble() * 1 + 1d);
            var verticalOffset = Rnd.NextDouble() + 4d;
            MouseControl.Move((int)(horizontalOffset), (int)(verticalOffset * AdjustmentCoefficient));

            return GetAdjustmentTime(1d);
        }
    }
}