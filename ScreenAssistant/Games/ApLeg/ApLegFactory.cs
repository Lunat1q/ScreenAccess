using System;
using System.Collections.Generic;
using System.Linq;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons;
using TiqSoft.ScreenAssistant.Helpers;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    internal static class ApLegFactory
    {
        private static readonly Dictionary<string, WeaponAL> WeaponNamesToTypes = new Dictionary<string, WeaponAL>(35);

        static ApLegFactory()
        {
            var values = Enum.GetValues(typeof(WeaponAL)).Cast<Enum>();
            foreach (var value in values)
            {
                if (value.GetType().GetField(value.ToString())
                    .GetCustomAttributes(typeof(WeaponNameAttribute), false).FirstOrDefault() is WeaponNameAttribute nameAttribute)
                {
                    WeaponNamesToTypes.Add(nameAttribute.Name.ToUpper(), (WeaponAL)value);
                    foreach (var extraName in nameAttribute.ExtraRecognitionNames)
                    {
                        WeaponNamesToTypes.Add(extraName.ToUpper(), (WeaponAL)value);
                    }
                }
            }
        }

        internal static IWeapon ConstructDefault()
        {
            return new RegularAdjustmentWeapon("Default", 2, "Default", WeaponAL.Unknown);
        }

        internal static IWeapon ConstructFromRecognizedString(string recognizedName, IWeapon currentWeapon, int offsetX, int offsetY)
        {
            var weaponName = recognizedName.FindMostSimilar(WeaponNamesToTypes.Keys);

            WeaponNamesToTypes.TryGetValue(weaponName, out var weaponType);

            var inGameName = weaponType.GetWeaponName();
            IWeapon result;

            if (currentWeapon?.IsTheSameWeapon(inGameName) ?? weaponType == WeaponAL.Unknown)
            {
                return currentWeapon;
            }

            switch (weaponType)
            {
                case WeaponAL.Wingman:
                case WeaponAL.MASTIFF:
                case WeaponAL.KRABER:
                case WeaponAL.Longbow:
                case WeaponAL.Peacekeeper:
                case WeaponAL.TripleTake:
                    result = new NonAdjustableWeapon(inGameName, 0, recognizedName, weaponType);
                    break;
                case WeaponAL.Havoc:
                    result = new Havoc(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.Devotion:
                    result = new Devotion(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.Flatline:
                    result = new Flatline(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.Hemlok:
                    result = new Hemlok(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.Prowler:
                    result = new Prowler(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.RE45:
                    result = new RE45(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.R301:
                    result = new R301(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.R99:
                    result = new R99(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.Alternator:
                    result = new Alternator(inGameName, 2, recognizedName);
                    break;
                case WeaponAL.Unknown:
                case WeaponAL.Spitfire:
                case WeaponAL.G7Scout:
                case WeaponAL.EVA8Auto:
                case WeaponAL.Mozambique:
                case WeaponAL.P2020:
                    result = new RegularAdjustmentWeapon(inGameName, 2, recognizedName, weaponType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            result.SetOffsets(offsetX, offsetY);
            return result;
        }



        
    }
}