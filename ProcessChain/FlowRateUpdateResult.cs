using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Результат обновления параметров схемы
    /// </summary>
    public class FlowRateUpdateResult
    {
        /// <summary>
        /// Ошибка обновления
        /// </summary>
        public FlowRestriction Restriction { get; private set; }

        public bool IsSuccess
        {
            get
            {
                return Restriction == null;
            }
        }

        public FlowRateUpdateResult(FlowRestriction restriction)
        {
            Restriction = restriction;
        }

        public FlowRateUpdateResult(): this(null)
        {

        }

        public override string ToString()
        {
            return $"IsSuccess: {IsSuccess}, Restriction: {Restriction?.ToString()}";
        }
    }
}
