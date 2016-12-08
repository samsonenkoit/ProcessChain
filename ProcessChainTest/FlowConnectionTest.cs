using NUnit.Framework;
using ProcessChain;
using ProcessChainTest.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChainTest
{
    [TestFixture]
    public class FlowConnectionTest
    {
        [Test]
        public void Constructor_ValidParameters_Success()
        {
            var stubQuota = new NodeConnectionQuota(1);
            var stubInput = new FakeFlowElement("1");
            var stubOutput = new FakeFlowElement("2");
            NodeConnection con = new NodeConnection("123", stubQuota, stubInput, stubOutput, 12);

            Assert.AreEqual(con.Id, "123");
            Assert.AreEqual(con.Quota, stubQuota);
            Assert.AreEqual(con.CurrentRate, 12);
            Assert.AreEqual(con.Input, stubInput);
            Assert.AreEqual(con.Output, stubOutput);
        }

        [Test]
        public void Constructor_InvalidParameters_Throw()
        {
            Assert.Throws<ArgumentNullException>(delegate { new NodeConnection("1", null, null, null); });
        }

        [Test]
        public void FlowRateUpdate_UpdateOutputFlowElement_Success()
        {
            var mockOutput = new FakeFlowElement("2");
            NodeConnection conn = new NodeConnection("1", new NodeConnectionQuota(12), new FakeFlowElement("1"), mockOutput);
            mockOutput.Connection = conn;
            conn.FlowRateUpdate(134);

            Assert.AreEqual(mockOutput.InputFlowRate["1"], 134);
        }
    }
}
