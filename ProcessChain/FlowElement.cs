using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Базовый класс для узла схемы
    /// </summary>
    public abstract class FlowElement: Element
    {
        /// <summary>
        /// Величина потока узла
        /// </summary>
        public abstract double FlowRate { get;  }

        public FlowElement(string id): base(id)
        {
        }
        
        internal abstract FlowRateUpdateResult UpdateFlowRates();
    }
}
