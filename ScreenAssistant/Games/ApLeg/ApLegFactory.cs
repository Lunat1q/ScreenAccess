﻿using System;
using System.Collections.Generic;
using System.Linq;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons;
using TiqSoft.ScreenAssistant.Helpers;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    [GameName("Apex Legends")]
    // ReSharper disable once UnusedMember.Global
    internal class ApLegFactory : IWeaponFactory
    {
        private static readonly Dictionary<string, WeaponAL> WeaponNamesToTypes = new Dictionary<string, WeaponAL>(40);
        private static readonly Dictionary<WeaponAL, int> WeaponTypesToNumOfMods = new Dictionary<WeaponAL, int>(25);

        static ApLegFactory()
        {
            var values = Enum.GetValues(typeof(WeaponAL)).Cast<Enum>();
            foreach (var value in values)
            {
                if (value.GetType().GetField(value.ToString())
                    .GetCustomAttributes(typeof(WeaponDataAttribute), false).FirstOrDefault() is WeaponDataAttribute nameAttribute)
                {
                    WeaponNamesToTypes.Add(nameAttribute.Name.ToUpper(), (WeaponAL)value);
                    foreach (var extraName in nameAttribute.ExtraRecognitionNames)
                    {
                        WeaponNamesToTypes.Add(extraName.ToUpper(), (WeaponAL)value);
                    }
                    WeaponTypesToNumOfMods.Add((WeaponAL)value, nameAttribute.NumberOfMods);
                }
            }
        }

        public ApLegFactory()
        {
            IntRecognizer = new ApLegWeaponTypeScreenRecognizer();
            Recognizer = IntRecognizer;
        }

        private ApLegWeaponTypeScreenRecognizer IntRecognizer { get; }

        private static IWeapon CreateDefaultWeapon()
        {
            return new RegularAdjustmentWeapon(string.Empty, 2, "Default", 0);
        }

        private static IWeapon CreateFromRecognizedString(string recognizedName, IWeapon currentWeapon, int offsetX, int offsetY, float sensitivityScale)
        {
            var weaponName = recognizedName.FindMostSimilar(WeaponNamesToTypes.Keys);

            WeaponNamesToTypes.TryGetValue(weaponName, out var weaponType);

            var inGameName = weaponType.GetWeaponName();
            IWeapon result;

            if (currentWeapon?.IsTheSameWeapon(inGameName) ?? weaponType == WeaponAL.Unknown)
            {
                return currentWeapon;
            }

            var numOfMods = WeaponTypesToNumOfMods[weaponType];
            switch (weaponType)
            {
                case WeaponAL.Wingman:
                case WeaponAL.MASTIFF:
                case WeaponAL.KRABER:
                case WeaponAL.Longbow:
                case WeaponAL.Peacekeeper:
                case WeaponAL.TripleTake:
                    result = new NonAdjustableWeapon(inGameName, 0, recognizedName, numOfMods);
                    break;
                case WeaponAL.Havoc:
                    result = new Havoc(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.Devotion:
                    result = new Devotion(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.LStar:
                    result = new LStar(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.Flatline:
                    result = new Flatline(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.Hemlok:
                    result = new Hemlok(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.Prowler:
                    result = new Prowler(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.RE45:
                    result = new RE45(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.R301:
                    result = new R301(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.R99:
                    result = new R99(inGameName, 1.5, recognizedName, numOfMods);
                    break;
                case WeaponAL.Alternator:
                    result = new Alternator(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.G7Scout:
                    result = new G7Scout(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.Unknown:
                case WeaponAL.Spitfire:
                case WeaponAL.EVA8Auto:
                case WeaponAL.Mozambique:
                case WeaponAL.P2020:
                    result = new RegularAdjustmentWeapon(inGameName, 2, recognizedName, numOfMods);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            result.SetOffsets(offsetX, offsetY);
            result.SetSensitivityScale(sensitivityScale);
            return result;
        }

        public IWeaponRecognizer Recognizer { get; }
        public IWeapon Default()
        {
            return CreateDefaultWeapon();
        }

        public IWeapon FromRecognizedString(string weaponString, IWeapon currentWeapon, int offsetX, int offsetY, float sensitivityScale)
        {
            return CreateFromRecognizedString(weaponString, currentWeapon, offsetX, offsetY, sensitivityScale);
        }

        public int NumberOfWeapons => 2;

        public void WeaponPostProcess(IWeapon weapon)
        {
            if (weapon.IsDefault()) return;
            var apexWeapon = (ApLegWeaponBase)weapon;
            var modulesState = IntRecognizer.GetModulesState(weapon.NumberOfModules);
            for (var i = 0; i < weapon.NumberOfModules; i++)
            {
                apexWeapon.InstalledModules[i].Type = modulesState[i];
            }
        }

        public string LockedToApplication { get; } = "r5apex";
    }
}