namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Prowler : UniqueLogicWeapon
    {
        public Prowler(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 14);
            var horizontalOffset = 0;
            var verticalOffset = Rnd.NextDouble() * 1 + 5.5d;
            MoveMouse(horizontalOffset, verticalOffset);
            return GetAdjustmentTime(0.3d);
        }
    }
}