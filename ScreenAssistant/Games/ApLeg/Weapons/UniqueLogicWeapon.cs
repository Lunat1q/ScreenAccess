﻿namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal abstract class UniqueLogicWeapon : ApLegWeaponBase
    {
        protected UniqueLogicWeapon(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
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