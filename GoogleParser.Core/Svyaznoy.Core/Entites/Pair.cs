using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Entites
{
    public class Pair<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public Pair()
        {
        }

        public Pair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
