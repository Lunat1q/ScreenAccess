using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Devotion : UniqueLogicWeapon
    {
        public Devotion(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            double horizontalOffset;
            double verticalOffset;
            var warmUpShort = 15;
            var extraVerticalAdjustment = 1;
            if (this.GetModuleType(4) != WeaponModuleType.Legendary)
            {
                warmUpShort = 0;
                extraVerticalAdjustment = 3;
            }
            if (shotNumber < warmUpShort)
            {
                this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 20);
                horizontalOffset = Rnd.NextDouble() * 1 + 1;
                verticalOffset = Rnd.NextDouble() * 1 + extraVerticalAdjustment + 5;
            }
            else
            {
                this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 80);
                var hAdj = shotNumber > 25 ? -1.5d : 1.5d; 
                horizontalOffset = hAdj * (Rnd.NextDouble() * 0.5 + 1);
                verticalOffset = Rnd.NextDouble() * 0.5 + 1.8d;
            }

            this.MoveMouse(horizontalOffset, verticalOffset);

            return this.GetAdjustmentTime(1d);
        }
    }
}