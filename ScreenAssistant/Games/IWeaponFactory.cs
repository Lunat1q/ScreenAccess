﻿using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games
{
    public interface IWeaponFactory
    {
        IWeaponRecognizer Recognizer { get; }

        IWeapon Default();

        IWeapon FromRecognizedString(string weaponString, IWeapon currentWeapon, int offsetX, int offsetY, float sensitivityScale);

        void WeaponPostProcess(IWeapon weapon);

        int NumberOfWeapons { get; }

        //Leave empty if no lock needed
        string LockedToApplication { get; }
    }
}