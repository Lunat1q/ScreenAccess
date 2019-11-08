using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class LStar : UniqueLogicWeapon
    {
        public LStar(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            double horizontalOffset;
            var verticalOffset = Rnd.NextDouble() * (2) + 5;
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 30);
            if (shotNumber < 7)
            {
                horizontalOffset = Rnd.NextDouble() * 1 + 1.5;
            }
            else if (shotNumber < 16)
            {
                horizontalOffset = -1 * (Rnd.NextDouble() * 1 + 2.25);
            }
            else
            {
                horizontalOffset = Rnd.NextDouble() * 1 + 1.5;
            }

            //Debug.WriteLine($"{shotNumber}, {horizontalOffset:F2}");

            this.MoveMouse(horizontalOffset, verticalOffset);

            return this.GetAdjustmentTime(1d);
        }
    }
}