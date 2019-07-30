
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
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 1);
            var fromLastShot = DateTime.Now - _lastShot;
            _lastShot = DateTime.Now;
            var msFromLastShot = fromLastShot.TotalMilliseconds;
            if (msFromLastShot < 150)
            {
                _fastShots++;
            }
            else if (msFromLastShot < 400)
            {
                _fastShots--;
            }
            else
            {
                _fastShots = 0;
            }

            var fastShotAdjustment = 1 - CalculateAdjustment(_fastShots, 12);

            var verticalOffset = (9d + -5d * fastShotAdjustment) * Math.Min(Math.Abs(1 - msFromLastShot / 600), 1);
            var horizontalOffset = (2.5d + 2d * fastShotAdjustment) * Math.Min(Math.Abs(1 - msFromLastShot / 300), 1);
            MoveMouse(horizontalOffset, verticalOffset);
            //Debug.WriteLine($"{_fastShots}: {verticalOffset:N2}, fsAdj: {fastShotAdjustment}");
            return GetAdjustmentTime(1d); 
        }
    }
}