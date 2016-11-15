using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Параметры соединения
    /// </summary>
    public class FlowConnectionQuota
    {
        #region Const

        public static double DirectValueNone = 0.0d;

        #endregion

        /// <summary>
        /// Процент общего потока, который должен подаваться в соединение
        /// </summary>
        public double Percent { get; private set; }
        /// <summary>
        /// Величина потока, который должен подаваться на соединение (Имеет приоритет перед процентом)
        /// </summary>
        public double DirectValue { get; private set; }

        /// <summary>
        /// True - если установленно DirectValue
        /// </summary>
        public bool IsDirectValue
        {
            get
            {
                return DirectValue != FlowConnectionQuota.DirectValueNone;
            }
        }

        public FlowConnectionQuota(double percent): this(percent, FlowConnectionQuota.DirectValueNone)
        {

        }

        public FlowConnectionQuota(double percent, double directValue)
        {
            if (percent < 0.0d) throw new ArgumentOutOfRangeException(nameof(percent));
            if (directValue < 0.0d) throw new ArgumentOutOfRangeException(nameof(directValue));

            Percent = percent;
            DirectValue = directValue;
        }

        public override string ToString()
        {
            return $"Percent: {Percent}, DirectValue: {DirectValue}";
        }

    }
}
