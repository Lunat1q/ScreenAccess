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

        public static class KnownGames
        {
            public const string ApLeg = "Apex Legends";
        }
    }
}