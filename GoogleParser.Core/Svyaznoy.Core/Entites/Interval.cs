namespace Svyaznoy.Core.Entites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Interval<T> where T : struct, IComparable
    {
        public Nullable<T> Start { get; set; }
        public Nullable<T> End { get; set; }

        public Interval() { }

        public Interval(Nullable<T> start, Nullable<T> end)
        {
            Start = start;
            End = end;
        }

        public bool InRange(T value)
        {
            return ((!Start.HasValue || value.CompareTo(Start.Value) > 0) &&
                    (!End.HasValue || End.Value.CompareTo(value) > 0));
        }
    }
}