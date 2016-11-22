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
    public class ConsumerTest
    {
        [Test]
        public void SerInputConnectionsTest()
        {
            var consumer = new Consumer("1");
            Assert.Throws<ArgumentNullException>(() => consumer.SetInputConnections(null));

            consumer.SetInputConnections(new List<NodeConnection>());

            Assert.Throws<InvalidOperationException>(() => consumer.SetInputConnections(new List<NodeConnection>()));
        }

        [Test]
        public void UpdateFlowRateTest()
        {
            var consumer = new Consumer("1");

            List<NodeConnection> input = new List<NodeConnection>()
            {
                new NodeConnection("c1", new NodeConnectionQuota(1), new FlowElementMock(), consumer, 1.10d),
                new NodeConnection("c2", new NodeConnectionQuota(1), new FlowElementMock(), consumer, 1.20d),
                new NodeConnection("c3", new NodeConnectionQuota(1), new FlowElementMock(), consumer, 1.30d)
            };

            consumer.SetInputConnections(input);

            var result = input[0].FlowRateUpdate(1.10d);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(consumer.FlowRate - 3.60d < 0.00001d, true);
        }


    }
}
