using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    // ReSharper disable once IdentifierTypo
    internal sealed class Flatline : UniqueLogicWeapon
    {
        // ReSharper disable once IdentifierTypo
        public Flatline(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 30);
            var vOffset = shotNumber < 7 ? 4d : 2d;
            var hAdj = shotNumber > 8 && shotNumber < 12 ? -1d : 1;
            var horizontalOffset = hAdj * (Rnd.NextDouble() * 1 + 1d);
            var verticalOffset = Rnd.NextDouble() + vOffset;
            this.MoveMouse(horizontalOffset, verticalOffset);

            return this.GetAdjustmentTime(1d);
        }
    }
}