using System.Runtime.InteropServices;

namespace TiqSoft.ScreenAssistant.Core
{
    internal static class MouseControl
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSE_EVENT_F_MOVE = 0x0001;

        public static void Move(int xDelta, int yDelta)
        {
            mouse_event(MOUSE_EVENT_F_MOVE, xDelta, yDelta, 0, 0);
        }
    }
}
