using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Model
{
    [Flags]
    public enum StringSearchOptions
    {
        None = 0,
        Left = 1,
        Right = 2,
        Contains = 4,
        Exact = 8,
    }
}
