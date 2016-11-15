using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Параметры узла
    /// </summary>
    public class FlowElementScope
    {
        /// <summary>
        /// Максимально возможный размер потока
        /// </summary>
        public double RateValueMax { get; private set; }

        public FlowElementScope(double rateValueMax)
        {
            if (rateValueMax <= 0.0d) throw new ArgumentOutOfRangeException(nameof(rateValueMax));

            RateValueMax = rateValueMax;
        }
    }
}
