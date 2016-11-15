using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Ичтоник вещества. Может иметь только один выходной поток.
    /// </summary>
    public class Extractor : FlowElement
    {
        private FlowConnection _outputConnection;
        

        private double _flowRate;

        public override double FlowRate
        {
            get
            {
                return _flowRate;
            }
        }

        public Extractor(string id) : base(id)
        {
        }

        /// <summary>
        /// Устанавливает выходной поток. Может иметь только один выходной поток
        /// </summary>
        /// <param name="connection"></param>
        internal void SetOutputConnection(FlowConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (_outputConnection != null) throw new InvalidOperationException("Connection is already existed");

            _outputConnection = connection;

        } 

        /// <summary>
        /// Устанавливает размер генерируемого потока
        /// </summary>
        /// <param name="flowRate"></param>
        /// <returns></returns>
        public FlowRateUpdateResult FlowRateUpdate(double flowRate)
        {
            if (flowRate < 0.0d) throw new ArgumentOutOfRangeException(nameof(flowRate));

            _flowRate = flowRate;
            return UpdateFlowRates();
        }

        internal override FlowRateUpdateResult UpdateFlowRates()
        {
            return _outputConnection.FlowRateUpdate(FlowRate);
        }
    }
}
