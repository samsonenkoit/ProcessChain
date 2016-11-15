using ProcessChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChainTest.Mock
{
    public class FlowElementMock : FlowElement
    {
        public Dictionary<string, double> InputFlowRate { get; set; } = new Dictionary<string, double>();

        public FlowConnection Connection { get; set; }

        public override double FlowRate
        {
            get
            {
                return InputFlowRate.Sum(t => t.Value);
            }
        }

        public FlowElementMock(): base(Guid.NewGuid().ToString())
        {

        }

        public FlowElementMock(string id) : base(id)
        {
        }

      /*  public override FlowRateUpdateResult UpdateOutputConnectionsQuota(IDictionary<string, FlowConnectionQuota> quotas)
        {
            throw new NotImplementedException();
        }*/

        internal override FlowRateUpdateResult UpdateFlowRates()
        {
            InputFlowRate[Connection.Id] = Connection.CurrentRate;

            return new FlowRateUpdateResult();
        }
    }
}
