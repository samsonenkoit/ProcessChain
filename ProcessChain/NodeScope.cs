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
    public class NodeScope
    {
        /// <summary>
        /// Максимально возможный размер потока
        /// </summary>
        public double RateValueMax { get; private set; }

        public NodeScope(double rateValueMax)
        {
            if (rateValueMax <= 0.0d) throw new ArgumentOutOfRangeException(nameof(rateValueMax));

            RateValueMax = rateValueMax;
        }
    }
}
