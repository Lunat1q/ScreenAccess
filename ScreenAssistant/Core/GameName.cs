using System;

namespace TiqSoft.ScreenAssistant.Core
{
    public class GameNameAttribute : Attribute
    {
        public GameNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}