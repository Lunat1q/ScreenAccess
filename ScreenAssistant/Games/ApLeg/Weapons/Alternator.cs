using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Alternator : UniqueLogicWeapon
    {
        public Alternator(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.Alternator)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 45);
            var horizontalOffset = Rnd.NextDouble() * 1 * 2 - 1;
            var verticalOffset = Rnd.NextDouble() + 5.5d;
            MouseControl.Move((int)horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
            return GetAdjustmentTime(1d);
        }
    }
}