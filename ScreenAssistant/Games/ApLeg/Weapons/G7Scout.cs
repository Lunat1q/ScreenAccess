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
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 1);
            var horizontalOffset = 0;
            var verticalOffset = Rnd.NextDouble()*3d + 7d;
            this.MoveMouse(horizontalOffset, verticalOffset);
            return this.GetAdjustmentTime(1d);
        }
    }
}