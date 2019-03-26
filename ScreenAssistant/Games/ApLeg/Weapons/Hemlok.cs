using TiqSoft.ScreenAssistant.Core;
// ReSharper disable IdentifierTypo

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Hemlok : UniqueLogicWeapon
    {
        public Hemlok(string name, double burstSeconds, string recognizedName) : base(name, burstSeconds, recognizedName, WeaponAL.Hemlok)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 5);
            var horizontalOffset = 0;
            var verticalOffset = Rnd.NextDouble() + 7d;
            MouseControl.Move(horizontalOffset, (int)(verticalOffset * AdjustmentCoefficient));

            return GetAdjustmentTime(1d);
        }
    }
}