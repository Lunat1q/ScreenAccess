namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    // ReSharper disable once IdentifierTypo
    internal sealed class Flatline : UniqueLogicWeapon
    {
        // ReSharper disable once IdentifierTypo
        public Flatline(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.Flatline)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 30);
            var vOffset = shotNumber < 7 ? 4d : 2d;
            var hAdj = shotNumber > 8 && shotNumber < 12 ? -1d : 1;
            var horizontalOffset = hAdj * (Rnd.NextDouble() * 1 + 1d);
            var verticalOffset = Rnd.NextDouble() + vOffset;
            MoveMouse(horizontalOffset, verticalOffset);

            return GetAdjustmentTime(1d);
        }
    }
}