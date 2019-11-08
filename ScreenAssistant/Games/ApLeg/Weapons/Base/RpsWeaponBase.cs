namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base
{
    internal abstract class RpsWeaponBase : ApLegWeaponBase
    {
        protected double Rps { get; set; }

        protected RpsWeaponBase(string name, double rps, string recognizedName, int numberOfModules) : base(name, recognizedName, numberOfModules)
        {
            this.Rps = rps;
        }

        protected double GetAdjustmentTime()
        {
            return 1 / this.Rps;
        }

        public override bool IsDefault()
        {
            return false;
        }
    }
}