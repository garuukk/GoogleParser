using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Model
{
    public class Limit
    {
        public int CountToSkip { get; set; }

        public int CountToTake { get; set; }

        public int TotalCount{ get; internal set; }

        public bool RequireTotalCount { get; set; }

        public Limit()
        {
        }

        public Limit(int countToTake, int countToSkip = 0, bool requireTotalCount= false)
        {
            CountToSkip = countToSkip;
            CountToTake = countToTake;
            RequireTotalCount = requireTotalCount;
        }
    }
}
