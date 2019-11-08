using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class NonAdjustableWeapon : ApLegWeaponBase
    {
        public NonAdjustableWeapon(string name, string recognizedName, int numOfMods) 
            : base(name, recognizedName, numOfMods)
        {
            this.AdjustmentCoefficient = 0;
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