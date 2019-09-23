namespace TiqSoft.ScreenAssistant.Core
{
    public class MouseMovedEventArgs
    {
        public MouseMovedEventArgs(int x, int y, int shotNum)
        {
            this.X = x;
            this.Y = y;
            this.ShotNum = shotNum;
        }

        public int X { get; }
        public int Y { get; }
        public int ShotNum { get; }
    }
}