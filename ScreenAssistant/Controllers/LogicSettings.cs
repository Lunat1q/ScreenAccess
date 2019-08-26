using TiqSoft.ScreenAssistant.Core.Settings;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal class LogicSettings
    {
        public int DeltaX { get; internal set; }

        public int DeltaY { get; internal set; }

        public bool UseWeaponLogic { get; internal set; }

        public bool LockToGameWindow { get; internal set; }

        public float SensitivityScale { get; internal set; }

        public bool FullScreenMode { get; internal set; }

        public static LogicSettings ConstructFromSettings(ScreenAssistantSettings settings)
        {
            return new LogicSettings
            {
                DeltaX = settings.DeltaX,
                DeltaY = settings.DeltaY,
                UseWeaponLogic = settings.UseUniqueWeaponLogic,
                LockToGameWindow = settings.LockToGameWindow,
                SensitivityScale = settings.SensitivityScale
            };
        }
    }
}