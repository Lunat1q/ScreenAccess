// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    // ReSharper disable once InconsistentNaming
    [Flags]
    public enum WeaponAL
    {
        [WeaponName("", " ")]
        Unknown = 1 << 0,
        [WeaponName("Havoc", "HAVUC")]
        Havoc = 1 << 1,
        [WeaponName("Triple take")]
        TripleTake = 1 << 2,
        [WeaponName("Devotion", "DEVUTIUN")]
        Devotion = 1 << 3,
        [WeaponName("Flatline")]
        Flatline = 1 << 4,
        [WeaponName("Hemlok", "HEMLUH")]
        Hemlok = 1 << 5,
        [WeaponName("Prowler", "DRUWLEP")]
        Prowler = 1 << 6,
        [WeaponName("Longbow", "LUNGBUW")]
        Longbow = 1 << 7,
        [WeaponName("Spitfire", "SDITFIRE")]
        Spitfire = 1 << 8,
        [WeaponName("Wingman")]
        Wingman = 1 << 9,
        [WeaponName("RE-45", "p E-z15")]
        RE45 = 1 << 10,
        [WeaponName("P2020", "P2o2o")]
        P2020 = 1 << 11,
        [WeaponName("R-301", "R-3 Ol", "9-501")]
        R301 = 1 << 12,
        [WeaponName("R-99", "9-99")]
        R99 = 1 << 13,
        [WeaponName("Alternator")]
        Alternator = 1 << 14,
        [WeaponName("G7 Scout", "G7SCOUT")]
        G7Scout = 1 << 15,
        [WeaponName("EVA-8 AUTO")]
        EVA8Auto = 1 << 16,
        [WeaponName("Peacekeeper", "PEACEHEEDER")]
        Peacekeeper = 1 << 17,
        [WeaponName("MOZAMBIQUE")]
        Mozambique = 1 << 18,
        [WeaponName("MASTIFF")]
        MASTIFF = 1 << 19,
        [WeaponName("KRABER", "HRABER")]
        KRABER = 1 << 20,
        [WeaponName("L-STAR", "LSTAR")]
        LStar = 1 << 21
    }

    public static class WeaponAlExtensions
    {
        public static bool HasFlagFast(this WeaponAL value, WeaponAL flag)
        {
            return (value & flag) != 0;
        }
    }
}
