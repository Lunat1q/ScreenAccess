using System.Runtime.InteropServices;

namespace TiqSoft.ScreenAssistant.Core
{
    internal static class MouseControl
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MouseEventFMove = 0x0001;

        public static void Move(int xDelta, int yDelta)
        {
            mouse_event(MouseEventFMove, xDelta, yDelta, 0, 0);
        }
    }
}
