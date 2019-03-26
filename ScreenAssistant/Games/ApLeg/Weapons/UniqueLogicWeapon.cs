namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal abstract class UniqueLogicWeapon : ApLegWeaponBase
    {
        protected UniqueLogicWeapon(string name, double burstSeconds, string recognizedName, WeaponAL type) 
            : base(name, burstSeconds, recognizedName, type)
        {
        }

        protected double GetAdjustmentTime(double timeCoefficient)
        {
            return timeCoefficient * (Rnd.NextDouble() * (MaxFireRate - MinFireRate) + MinFireRate);
        }

        public override bool IsDefault()
        {
            return false;
        }
    }
}