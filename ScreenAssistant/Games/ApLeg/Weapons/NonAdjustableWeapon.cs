namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class NonAdjustableWeapon : ApLegWeaponBase
    {
        public NonAdjustableWeapon(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
            AdjustmentCoefficient = 0;
        }

        public override double AdjustMouse(int shotNumber)
        {
            return 1d;
        }

        public override bool IsDefault()
        {
            return false;
        }
    }
}