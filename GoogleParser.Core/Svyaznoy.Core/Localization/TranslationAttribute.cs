using System;
using System.Linq;

namespace Svyaznoy.Core.Localization
{
    public class TranslationAttribute: Attribute
    {
        public string Key { get; private set; }

        public TranslationAttribute(string key)
        {
            Key = key;
        }
    }
}
