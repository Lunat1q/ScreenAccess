
// ReSharper disable IdentifierTypo

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Hemlok : UniqueLogicWeapon
    {
        public Hemlok(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 5);
            var horizontalOffset = 0;
            var verticalOffset = Rnd.NextDouble() + 7d;
            MoveMouse(horizontalOffset, verticalOffset);

            return GetAdjustmentTime(1d);
        }
    }
}