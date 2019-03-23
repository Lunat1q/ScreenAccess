using System;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition
{
    public class WeaponNameAttribute : Attribute
    {
        public string Name { get; }
        public string[] ExtraRecognitionNames { get; }

        public WeaponNameAttribute(string name, params string[] extraRecognitionNames)
        {
            Name = name;
            this.ExtraRecognitionNames = extraRecognitionNames ?? new string[0];
        }
    }
}
