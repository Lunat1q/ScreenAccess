using System;
using System.Collections.Generic;
using System.Linq;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons;
using TiqSoft.ScreenAssistant.Games.ApLeg.Weapons.Base;
using TiqSoft.ScreenAssistant.Helpers;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    [GameName(GameNameAttribute.KnownGames.ApLeg)]
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
            this.IntRecognizer = new ApLegWeaponTypeScreenRecognizer();
            this.Recognizer = this.IntRecognizer;
        }

        private ApLegWeaponTypeScreenRecognizer IntRecognizer { get; }

        private static IWeapon CreateDefaultWeapon()
        {
            return new RegularAdjustmentWeapon(string.Empty, 2, "Default", 0);
        }

        private IWeapon CreateFromRecognizedString(string recognizedName, IWeapon currentWeapon,float sensitivityScale)
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
                case WeaponAL.ChargedRifle:
                    result = new NonAdjustableWeapon(inGameName, recognizedName, numOfMods);
                    break;
                case WeaponAL.Havoc:
                    result = new Havoc(inGameName, 2, recognizedName, numOfMods);
                    break;
                case WeaponAL.Devotion:
                    result = new Devotion(inGameName, recognizedName, numOfMods);
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
                case WeaponAL.Spitfire:
                    result = new Spitfire(inGameName, recognizedName, numOfMods);
                    break;
                case WeaponAL.Unknown:
                case WeaponAL.EVA8Auto:
                case WeaponAL.Mozambique:
                case WeaponAL.P2020:
                    result = new RegularAdjustmentWeapon(inGameName, 2, recognizedName, numOfMods);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            result.SetSensitivityScale(sensitivityScale);
            this.OnWeaponCreated(result);
            return result;
        }

        public IWeaponRecognizer Recognizer { get; }

        public IWeapon Default()
        {
            return CreateDefaultWeapon();
        }

        public IWeapon FromRecognizedString(string weaponString, IWeapon currentWeapon, float sensitivityScale)
        {
            return this.CreateFromRecognizedString(weaponString, currentWeapon, sensitivityScale);
        }

        public int NumberOfWeapons => 2;

        public void WeaponPostProcess(IWeapon weapon)
        {
            if (weapon.IsDefault()) return;
            var apexWeapon = (ApLegWeaponBase)weapon;
            var modulesState = this.IntRecognizer.GetModulesState(weapon.NumberOfModules);
            for (var i = 0; i < weapon.NumberOfModules; i++)
            {
                /*
                 * temp solution to prevent flickering, considering out of scope scenario when you remove a module from a weapon
                 * Common / None mods are quite similar as color, still looking for a way to detect them a bit more precisely. 
                */
                if (modulesState[i] != WeaponModuleType.None || apexWeapon.InstalledModules[i].Type == WeaponModuleType.Common) 
                {
                    apexWeapon.SetModule(i, modulesState[i]);
                }
            }
        }

        public string LockedToApplication { get; } = "r5apex";

        public event WeaponCreatedEvent WeaponCreated;

        protected virtual void OnWeaponCreated(IWeapon weapon)
        {
            this.WeaponCreated?.Invoke(this, new WeaponCreatedEventArgs(weapon));
        }
    }
}