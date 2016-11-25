using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Extension
{
    public static class DoubleExtension
    {
        public static bool Equals(this double val1, double val2, double maxDifference)
        {
            return Math.Abs(val1 - val2) < maxDifference;
        }
    }
}
