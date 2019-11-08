using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Havoc : UniqueLogicWeapon
    {
        private bool _charged;

        public Havoc(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override void SetModule(int id, WeaponModuleType moduleType)
        {
            base.SetModule(id, moduleType);
            if (id == 3)
            {
                this._charged = moduleType == WeaponModuleType.Legendary;
            }
        }

        public override double AdjustMouse(int shotNumber)
        {
            var notOffsettingDelay = this._charged ? 0 : 6;
            if (shotNumber > notOffsettingDelay)
            {
                this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 40);
                var horizontalOffset = Rnd.NextDouble() * 2 - 1;
                var verticalOffset = Rnd.NextDouble() + 5.5d;
                this.MoveMouse(horizontalOffset, verticalOffset);
            }

            return this.GetAdjustmentTime(1d);
        }
    }
}