namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class G7Scout : UniqueLogicWeapon
    {
        public G7Scout(string name, double burstSeconds, string recognizedName, int numOfMods)
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 1);
            var horizontalOffset = 0;
            var verticalOffset = Rnd.NextDouble()*3d + 7d;
            MoveMouse(horizontalOffset, verticalOffset);
            return GetAdjustmentTime(1d);
        }
    }
}