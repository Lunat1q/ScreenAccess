using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Prowler : UniqueLogicWeapon
    {
        public Prowler(string name, double burstSeconds, string recognizedName) : base(name, burstSeconds, recognizedName, WeaponAL.Prowler)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 14);
            var horizontalOffset = 0;
            var verticalOffset = Rnd.NextDouble() * 1 + 5.5d;
            MouseControl.Move(horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));
            return GetAdjustmentTime(0.3d);
        }
    }
}