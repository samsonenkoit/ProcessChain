using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    public class Consumer : FlowElement
    {
        private Dictionary<string, FlowConnection> _inputConnections;


        private double _flowRate;
        public override double FlowRate
        {
            get
            {
                return _flowRate;
            }
        }

        public Consumer(string id) : base(id)
        {
        }
        
        internal void SetInputConnections(IEnumerable<FlowConnection> connections)
        {
            if (connections == null) throw new ArgumentNullException(nameof(connections));
            if (_inputConnections != null) throw new InvalidOperationException("Connections is already existed");

            _inputConnections = new Dictionary<string, FlowConnection>();

            foreach (var con in connections)
                _inputConnections.Add(con.Id, con);
        }

        internal override FlowRateUpdateResult UpdateFlowRates()
        {
            _flowRate = _inputConnections.Sum(t => t.Value.CurrentRate);
            return new FlowRateUpdateResult();
        }
    }
}
