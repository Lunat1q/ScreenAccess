using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    // ReSharper disable once InconsistentNaming
    internal sealed class RE45 : UniqueLogicWeapon
    {
        public RE45(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 25);
            var horizontalOffset = -1 * (Rnd.NextDouble() * 1 + 2d);
            var verticalOffset = Rnd.NextDouble() + 4d;
            this.MoveMouse(horizontalOffset, verticalOffset);

            return this.GetAdjustmentTime(1d);
        }
    }
}