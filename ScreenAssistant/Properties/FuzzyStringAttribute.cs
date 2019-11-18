using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiqSoft.ScreenAssistant.Properties
{
    internal sealed class FuzzyStringAttribute : Attribute
    {
        private string _text;

        public FuzzyStringAttribute(string text)
        {
            this._text = text;
        }
    }
}
