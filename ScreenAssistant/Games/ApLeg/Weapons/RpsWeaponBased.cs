namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal abstract class RpsWeaponBased : ApLegWeaponBase
    {
        protected double Rps { get; set; }

        protected RpsWeaponBased(string name, double rps, string recognizedName, int numberOfModules) : base(name, 0, recognizedName, numberOfModules)
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