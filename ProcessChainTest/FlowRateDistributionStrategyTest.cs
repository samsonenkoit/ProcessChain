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
    public class FlowRateDistributionStrategyTest
    {
        private const double MaxError = 0.001;

        #region Helper

        private NodeConnection BuildConnection(string id, NodeConnectionQuota quota, double flowRate)
        {
            return new NodeConnection(id, quota, new FakeFlowElement("noId"), new FakeFlowElement("noId"), flowRate);
        }

        #endregion

        [Test]
        public void MaxByQuotes_InvalidParameters_Throw()
        {
            Func<IEnumerable<NodeConnection>, IEnumerable<NodeConnection>, IDictionary<string, double>> func = FlowRateDistributionStrategy.MaxByQuotes;

            Assert.Throws<ArgumentOutOfRangeException>(delegate { func(null, null); });
            Assert.Throws<ArgumentOutOfRangeException>(delegate { func(new List<NodeConnection>(), new List<NodeConnection>()); });
        }

        [Test]
        public void MaxByQuotes_Scheme1Distribution_Success()
        {
            List<NodeConnection> inputConnections = new List<NodeConnection>()
            {
                BuildConnection("1", new NodeConnectionQuota(0), 30),
                BuildConnection("2", new NodeConnectionQuota(0), 20),
                BuildConnection("3", new NodeConnectionQuota(0), 50)
            };

            List<NodeConnection> outputConnections = new List<NodeConnection>()
            {
                BuildConnection("4", new NodeConnectionQuota(10), 0),
                BuildConnection("5", new NodeConnectionQuota(60), 0),
                BuildConnection("6", new NodeConnectionQuota(30), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(newRates["4"], 10.0d);
            Assert.AreEqual(newRates["5"], 60.0d);
            Assert.AreEqual(newRates["6"], 30.0d);
        }

        [Test]
        public void MaxByQuotes_Scheme2Distribution_Success()
        {
            List<NodeConnection> inputConnections = new List<NodeConnection>()
            {
                BuildConnection("1", new NodeConnectionQuota(0), 30),
                BuildConnection("2", new NodeConnectionQuota(0), 20),
                BuildConnection("3", new NodeConnectionQuota(0), 50),
                BuildConnection("3", new NodeConnectionQuota(0), 50),
                BuildConnection("3", new NodeConnectionQuota(0), 50)
            };

            List<NodeConnection> outputConnections = new List<NodeConnection>()
            {
                BuildConnection("4", new NodeConnectionQuota(55.7), 0),
                BuildConnection("5", new NodeConnectionQuota(40), 0),
                BuildConnection("6", new NodeConnectionQuota(4.3), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(newRates["4"], 111.4d);
            Assert.AreEqual(newRates["5"], 80.0d);
            Assert.AreEqual(newRates["6"], 8.6d);
        }

        [Test]
        public void MaxByQuotes_Scheme3Distribution_Success()
        {
            List<NodeConnection> inputConnections = new List<NodeConnection>()
            {
                BuildConnection("1", new NodeConnectionQuota(0), 30),
                BuildConnection("2", new NodeConnectionQuota(0), 20),
                BuildConnection("3", new NodeConnectionQuota(0), 50),
                BuildConnection("3", new NodeConnectionQuota(0), 50),
                BuildConnection("3", new NodeConnectionQuota(0), 50)
            };

            List<NodeConnection> outputConnections = new List<NodeConnection>()
            {
                BuildConnection("4", new NodeConnectionQuota(10, 40), 0),
                BuildConnection("5", new NodeConnectionQuota(20), 0),
                BuildConnection("6", new NodeConnectionQuota(30, 10), 0),
                BuildConnection("7", new NodeConnectionQuota(25), 0),
                BuildConnection("8", new NodeConnectionQuota(25), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(Math.Abs(newRates["4"] - 40.0d) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["5"] - 42.857142) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["6"] - 10.0d) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["7"] - 53.57142) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["8"] - 53.57142) < MaxError, true);
        }

        [Test]
        public void MaxByQuotes_Scheme4Distribution_Success()
        {
            List<NodeConnection> inputConnections = new List<NodeConnection>()
            {
                BuildConnection("1", new NodeConnectionQuota(0), 30),
                BuildConnection("2", new NodeConnectionQuota(0), 20)
            };

            List<NodeConnection> outputConnections = new List<NodeConnection>()
            {
                BuildConnection("4", new NodeConnectionQuota(130), 0),
                BuildConnection("5", new NodeConnectionQuota(130), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(newRates["4"], 25.0d);
            Assert.AreEqual(newRates["5"], 25.0d);
        }
    }
}
