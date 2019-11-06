namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Spitfire : RpsWeaponBased
    {
        private const double SpitfireRps = 12.5;
        public Spitfire(string name, string recognizedName, int numOfMods) 
            : base(name, SpitfireRps, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 65);

            double horizontalOffset;
            if (shotNumber <= 3)
            {
                horizontalOffset = Rnd.NextDouble() * 0.4d + 0.25d;
            }
            else if (shotNumber <= 7)
            {
                horizontalOffset = Rnd.NextDouble() + 0.75d;
            }
            else if (shotNumber <= 12)
            {
                horizontalOffset = -1 * (Rnd.NextDouble() + 2.35d);
            }
            else if (shotNumber < 22)
            {
                horizontalOffset = Rnd.NextDouble() * 0.5d + 5.25d;
            }
            else
            {
                horizontalOffset = -1 * (Rnd.NextDouble() * 0.5d + 3.1d);
            }

            double verticalOffset;
            if (shotNumber <= 5)
            {
                verticalOffset = Rnd.NextDouble() * 0.5d + 5.25d;
            }
            else if (shotNumber <= 15)
            {
                verticalOffset = Rnd.NextDouble() * 0.5d + 2.45d;
            }
            else if (shotNumber <= 22)
            {
                verticalOffset = 1;
            }
            else
            {
                verticalOffset = Rnd.NextDouble() * 0.5d + 1.35d;
            }

            this.MoveMouse(horizontalOffset, verticalOffset);

            return this.GetAdjustmentTime();
        }
    }
}