using System;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends
{
    [Flags]
    public enum WeaponModuleType
    {
        None = 0,
        Common = 2,
        Rare = 4,
        Epic = 8,
        Legendary = 16
    }
}