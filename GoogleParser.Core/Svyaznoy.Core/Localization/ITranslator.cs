using System;
using System.Linq;

namespace Svyaznoy.Core.Localization
{
    public interface ITranslator
    {
        string TranslateValue(string key);
    }
}
