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
        private Consumer GetDefaulteConsumer()
        {
            return new Consumer("1");
        }

        [Test]
        public void SetInputConnections_NullParameter_Throw()
        {
            var consumer = GetDefaulteConsumer();

            Assert.Throws<ArgumentNullException>(() => consumer.SetInputConnections(null));

        }


        [Test]
        public void SetInputConnections_SecondConnectionsSet_Throw()
        {
            var consumer = GetDefaulteConsumer();

            consumer.SetInputConnections(new List<NodeConnection>());

            Assert.Throws<InvalidOperationException>(() => consumer.SetInputConnections(new List<NodeConnection>()));
        }

        [Test]
        public void FlowRateUpdate_ConsumerUpdateFlowRate_Success()
        {
            var consumer = GetDefaulteConsumer();

            List<NodeConnection> stubInputConnections = new List<NodeConnection>()
            {
                new NodeConnection("c1", new NodeConnectionQuota(1), new FakeFlowElement(), consumer, 1.10d),
                new NodeConnection("c2", new NodeConnectionQuota(1), new FakeFlowElement(), consumer, 1.20d),
                new NodeConnection("c3", new NodeConnectionQuota(1), new FakeFlowElement(), consumer, 1.30d)
            };

            consumer.SetInputConnections(stubInputConnections);

            var result = stubInputConnections[0].FlowRateUpdate(1.10d);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(consumer.FlowRate - 3.60d < 0.00001d, true);
        }


    }
}
