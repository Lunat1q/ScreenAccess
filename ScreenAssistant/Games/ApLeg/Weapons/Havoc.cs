namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Havoc : UniqueLogicWeapon
    {
        public Havoc(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.Havoc)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            if (shotNumber > 6)
            {
                AdjustmentCoefficient = CalculateAdjustment(shotNumber, 40);
                var horizontalOffset = Rnd.NextDouble() * 1 * 2 - 1;
                var verticalOffset = Rnd.NextDouble() + 5.5d;
                MoveMouse(horizontalOffset, verticalOffset);
            }

            return GetAdjustmentTime(1d);
        }
    }
}