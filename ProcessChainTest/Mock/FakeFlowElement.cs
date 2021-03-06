﻿using ProcessChain;
using ProcessChain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChainTest.Mock
{
    public class FakeFlowElement : NodeElement
    {
        public Dictionary<string, double> InputFlowRate { get; set; } = new Dictionary<string, double>();

        public NodeConnection Connection { get; set; }

        public override double FlowRate
        {
            get
            {
                return InputFlowRate.Sum(t => t.Value);
            }
        }

        public FakeFlowElement(): base(Guid.NewGuid().ToString())
        {

        }

        public FakeFlowElement(string id) : base(id)
        {
        }

      /*  public override FlowRateUpdateResult UpdateOutputConnectionsQuota(IDictionary<string, FlowConnectionQuota> quotas)
        {
            throw new NotImplementedException();
        }*/

        internal override SchemeRateUpdateResult UpdateFlowRates()
        {
            InputFlowRate[Connection.Id] = Connection.CurrentRate;

            return new SchemeRateUpdateResult();
        }
    }
}
