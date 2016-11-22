using ProcessChain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Interface
{
    /// <summary>
    /// Базовый класс для узла схемы
    /// </summary>
    public abstract class NodeElement: Element
    {
        /// <summary>
        /// Величина потока узла
        /// </summary>
        public abstract double FlowRate { get;  }

        public NodeElement(string id): base(id)
        {
        }
        
        internal abstract SchemeRateUpdateResult UpdateFlowRates();
    }
}
