using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public class VS_MinMax
    {
        public int min = 0;
        public int max = 0;

        public VS_MinMax()
        {
        }

        public VS_MinMax(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            if (min == max)
                return min.ToString();
            return min + "-" + max;
        }

        public void Reset()
        {
            min = 0;
            max = 0;
        }

        public bool Empty()
        {
            return (min == 0 && max == 0);
        }

        public static VS_MinMax operator +(VS_MinMax a, VS_MinMax b)
        {
            return new VS_MinMax(a.min + b.min, a.max + b.max);
        }

    }
}
