using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Localization
{
    public class DefaultTranslator: ITranslator
    {
        public string TranslateValue(string key)
        {
            return key;
        }
    }
}
