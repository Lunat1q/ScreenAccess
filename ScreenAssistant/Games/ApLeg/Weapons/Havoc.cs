using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Havoc : UniqueLogicWeapon
    {
        public Havoc(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            var notOffsettingDelay = this.GetModuleType(4) == WeaponModuleType.Legendary ? 0 : 6;
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