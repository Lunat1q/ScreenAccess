using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games
{
    public class WeaponCreatedEventArgs
    {
        public WeaponCreatedEventArgs(IWeapon weapon)
        {
            Weapon = weapon;
        }

        public IWeapon Weapon { get; }
    }
}