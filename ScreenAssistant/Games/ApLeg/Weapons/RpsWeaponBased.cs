namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal abstract class RpsWeaponBased : ApLegWeaponBase
    {
        private readonly double _rps;

        protected RpsWeaponBased(string name, double rps, string recognizedName, int numberOfModules) : base(name, 0, recognizedName, numberOfModules)
        {
            this._rps = rps;
        }

        protected double GetAdjustmentTime()
        {
            return 1 / this._rps;
        }

        public override bool IsDefault()
        {
            return false;
        }
    }
}