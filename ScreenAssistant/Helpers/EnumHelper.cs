using System.Linq;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;

namespace TiqSoft.ScreenAssistant.Helpers
{
    internal static class EnumHelper
    {
        internal static string GetWeaponName(this WeaponAL weapon)
        {
            var type = typeof(WeaponAL);
            var memInfo = type.GetMember(weapon.ToString());
            if (memInfo[0].GetCustomAttributes(typeof(WeaponNameAttribute), false).FirstOrDefault() is WeaponNameAttribute attribute)
            {
                return attribute.Name;
            }

            return "";
        }
    }
}
