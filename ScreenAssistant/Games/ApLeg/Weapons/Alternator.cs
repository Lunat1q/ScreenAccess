using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Alternator : UniqueLogicWeapon
    {
        public Alternator(string name, double burstSeconds, string recognizedName, int numOfMods)
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 25);
            var horizontalOffset = Rnd.NextDouble() * 1 * 2 - 1;
            var verticalOffset = Rnd.NextDouble() + 5.4d;
            this.MoveMouse(horizontalOffset, verticalOffset);
            return this.GetAdjustmentTime(1d);
        }
    }
}