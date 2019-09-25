
// ReSharper disable IdentifierTypo

using System;
using System.Diagnostics;

namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class Hemlok : UniqueLogicWeapon
    {
        private DateTime _lastShot = DateTime.MinValue;
        private int _fastShots;

        public Hemlok(string name, double burstSeconds, string recognizedName, int numOfMods) 
            : base(name, burstSeconds, recognizedName, numOfMods)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            this.AdjustmentCoefficient = CalculateAdjustment(shotNumber, 1);
            var fromLastShot = DateTime.Now - this._lastShot;
            this._lastShot = DateTime.Now;
            var msFromLastShot = fromLastShot.TotalMilliseconds;
            if (msFromLastShot < 150)
            {
                this._fastShots++;
            }
            else if (msFromLastShot < 400)
            {
                this._fastShots--;
            }
            else
            {
                this._fastShots = 0;
            }

            var fastShotAdjustment = 1 - CalculateAdjustment(this._fastShots, 12);

            var verticalOffset = (9d + -5d * fastShotAdjustment) * Math.Min(Math.Abs(1 - msFromLastShot / 600), 1);
            var horizontalOffset = (2.5d + 2d * fastShotAdjustment) * Math.Min(Math.Abs(1 - msFromLastShot / 300), 1);
            this.MoveMouse(horizontalOffset, verticalOffset);
            //Debug.WriteLine($"{_fastShots}: {verticalOffset:N2}, fsAdj: {fastShotAdjustment}");
            return this.GetAdjustmentTime(1d); 
        }
    }
}