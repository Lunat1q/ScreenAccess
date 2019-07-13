namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Devotion : UniqueLogicWeapon
    {
        public Devotion(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            double horizontalOffset;
            double verticalOffset;
            if (shotNumber < 15)
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 20);
                horizontalOffset = Rnd.NextDouble() * 1 + 1;
                verticalOffset = Rnd.NextDouble() * (2) + 5;
            }
            else
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 80);
                var hAdj = shotNumber > 25 ? -1.5d : 1.5d; 
                horizontalOffset = hAdj * (Rnd.NextDouble() * 0.5 + 1);
                verticalOffset = Rnd.NextDouble() * 0.5 + 2d;
            }

            MoveMouse(horizontalOffset, verticalOffset);

            return GetAdjustmentTime(1d);
        }
    }
}