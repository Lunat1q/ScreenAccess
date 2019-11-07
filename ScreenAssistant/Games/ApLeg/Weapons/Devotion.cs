using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Devotion : RpsWeaponBased
    {
        private const double DevotionRps = 18d;
        private const double DevotionChargedRps = 25d;
        private bool _isCharged = false;
        public Devotion(string name, string recognizedName, int numOfMods) 
            : base(name, DevotionRps, recognizedName, numOfMods)
        {
        }

        public override void SetModule(int id, WeaponModuleType moduleType)
        {
            base.SetModule(id, moduleType);
            if (id == 4)
            {
                this._isCharged = moduleType == WeaponModuleType.Legendary;
                this.Rps = this._isCharged ? DevotionChargedRps : DevotionRps;
            }
        }

        public override double AdjustMouse(int shotNumber)
        {
            double horizontalOffset;
            double verticalOffset;
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 80);
            if (shotNumber <= 10)
            {
                horizontalOffset = 0;
                verticalOffset = Rnd.NextDouble() + 7;
            }
            else if (shotNumber <= 15)
            {
                verticalOffset = Rnd.NextDouble() + 2;
                horizontalOffset = Rnd.NextDouble() + 4;
            }
            else if (shotNumber <= 18)
            {
                verticalOffset = 0;
                horizontalOffset = Rnd.NextDouble() + 3;
            }
            else if (shotNumber <= 20)
            {
                verticalOffset = Rnd.NextDouble() + 2;
                horizontalOffset = 0;
            }
            else if (shotNumber <= 22)
            {
                verticalOffset = Rnd.NextDouble() + 2;
                horizontalOffset = -1 * (Rnd.NextDouble() + 1);
            }
            else if (shotNumber <= 25)
            {
                verticalOffset = Rnd.NextDouble() + 2;
                horizontalOffset = 0;
            }
            else
            {
                verticalOffset = Rnd.NextDouble() + 2;
                horizontalOffset = -1 * (Rnd.NextDouble() + 2);
            }

            this.MoveMouse(horizontalOffset, verticalOffset);

            return this.GetAdjustmentTime();
        }
    }
}