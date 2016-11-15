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

            consumer.SetInputConnections(new List<FlowConnection>());

            Assert.Throws<InvalidOperationException>(() => consumer.SetInputConnections(new List<FlowConnection>()));
        }

        [Test]
        public void UpdateFlowRateTest()
        {
            var consumer = new Consumer("1");

            List<FlowConnection> input = new List<FlowConnection>()
            {
                new FlowConnection("c1", new FlowConnectionQuota(1), new FlowElementMock(), consumer, 1.10d),
                new FlowConnection("c2", new FlowConnectionQuota(1), new FlowElementMock(), consumer, 1.20d),
                new FlowConnection("c3", new FlowConnectionQuota(1), new FlowElementMock(), consumer, 1.30d)
            };

            consumer.SetInputConnections(input);

            var result = input[0].FlowRateUpdate(1.10d);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(consumer.FlowRate - 3.60d < 0.00001d, true);
        }


    }
}
