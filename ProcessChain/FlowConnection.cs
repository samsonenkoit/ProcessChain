using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Элемент соединяющий два объекта узла
    /// </summary>
    public class FlowConnection: Element
    {

        /// <summary>
        /// Входной узел
        /// </summary>
        public FlowElement Input { get; private set; }
        /// <summary>
        /// Выходной узел
        /// </summary>
        public FlowElement Output { get; private set; }

        private FlowConnectionQuota _quota;
        /// <summary>
        /// Параметры соединения
        /// </summary>
        public FlowConnectionQuota Quota
        {
            get
            {
                return _quota;
            }
            internal set
            {
                if (value == null) throw new ArgumentNullException();

                _quota = value;
            }
        }

        /// <summary>
        /// Текущий поток соединения
        /// </summary>
        public double CurrentRate { get; private set; }

        internal FlowConnection(string id, FlowConnectionQuota quota, FlowElement input, FlowElement output) : this(id, quota, input, output, 0.0d)
        {

        }

        internal FlowConnection(string id, FlowConnectionQuota quota, FlowElement input, FlowElement output, double currentValue) : base(id)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));

            Input = input;
            Output = output;
            Quota = quota;
            CurrentRate = currentValue;
        }

        internal FlowRateUpdateResult FlowRateUpdate(double flowRate)
        {
            if (flowRate < 0.0d) throw new ArgumentOutOfRangeException(nameof(flowRate));

            CurrentRate = flowRate;
            return Output.UpdateFlowRates();
        } 

    }
}
