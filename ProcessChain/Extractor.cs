using ProcessChain.Interface;
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
    public class Extractor : NodeElement
    {
        private NodeConnection _outputConnection;
        

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
        internal void SetOutputConnection(NodeConnection connection)
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
        public SchemeRateUpdateResult FlowRateUpdate(double flowRate)
        {
            if (flowRate < 0.0d) throw new ArgumentOutOfRangeException(nameof(flowRate));

            _flowRate = flowRate;
            return UpdateFlowRates();
        }

        internal override SchemeRateUpdateResult UpdateFlowRates()
        {
            return _outputConnection.FlowRateUpdate(FlowRate);
        }
    }
}
