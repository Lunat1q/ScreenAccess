using TiqSoft.ScreenAssistant.Core.Settings;

namespace TiqSoft.ScreenAssistant.Games
{
    internal interface ISettingsReader
    {
        void UpdateSettings(ScreenAssistantSettings settings);
    }
}