namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base
{
    internal abstract class FireBurstRateWeaponBase : ApLegWeaponBase
    {

        protected const double MinFireRate = 0.04;
        protected const double MaxFireRate = 0.05;
        protected static readonly double ShotsPerSecond = 1 / ((MaxFireRate + MinFireRate) / 2);
        protected readonly double ShotsPerBurst;

        protected FireBurstRateWeaponBase(string name, double burstSeconds, string recognizedName, int numberOfModules) : base(name, recognizedName, numberOfModules)
        {
            this.ShotsPerBurst = ShotsPerSecond * burstSeconds;
        }
    }
}