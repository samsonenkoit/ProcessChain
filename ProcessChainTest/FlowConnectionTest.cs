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
        public void ConstructorTest()
        {
            var quota = new FlowConnectionQuota(1);
            var input = new FlowElementMock("1");
            var output = new FlowElementMock("2");
            FlowConnection con = new FlowConnection("123", quota, input, output, 12);

            Assert.AreEqual(con.Id, "123");
            Assert.AreEqual(con.Quota, quota);
            Assert.AreEqual(con.CurrentRate, 12);
            Assert.AreEqual(con.Input, input);
            Assert.AreEqual(con.Output, output);
        }

        [Test]
        public void ConstructorParameterValidationTest()
        {
            Assert.Throws<ArgumentNullException>(delegate { new FlowConnection("1", null, null, null); });
        }

        [Test]
        public void FlowRateUpdateTest()
        {
            var output = new FlowElementMock("2");
            FlowConnection conn = new FlowConnection("1", new FlowConnectionQuota(12), new FlowElementMock("1"), output);
            output.Connection = conn;
            conn.FlowRateUpdate(134);

            Assert.AreEqual(output.InputFlowRate["1"], 134);
        }
    }
}
