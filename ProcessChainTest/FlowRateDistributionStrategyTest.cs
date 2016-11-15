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

        private FlowConnection BuildConnection(string id, FlowConnectionQuota quota, double flowRate)
        {
            return new FlowConnection(id, quota, new FlowElementMock("noId"), new FlowElementMock("noId"), flowRate);
        }

        #endregion

        [Test]
        public void ConstructorInputParamsValidationTest()
        {
            Func<IEnumerable<FlowConnection>, IEnumerable<FlowConnection>, IDictionary<string, double>> func = FlowRateDistributionStrategy.MaxByQuotes;

            Assert.Throws<ArgumentOutOfRangeException>(delegate { func(null, null); });
            Assert.Throws<ArgumentOutOfRangeException>(delegate { func(new List<FlowConnection>(), new List<FlowConnection>()); });
        }

        [Test]
        public void DistributionTest1()
        {
            List<FlowConnection> inputConnections = new List<FlowConnection>()
            {
                BuildConnection("1", new FlowConnectionQuota(0), 30),
                BuildConnection("2", new FlowConnectionQuota(0), 20),
                BuildConnection("3", new FlowConnectionQuota(0), 50)
            };

            List<FlowConnection> outputConnections = new List<FlowConnection>()
            {
                BuildConnection("4", new FlowConnectionQuota(10), 0),
                BuildConnection("5", new FlowConnectionQuota(60), 0),
                BuildConnection("6", new FlowConnectionQuota(30), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(newRates["4"], 10.0d);
            Assert.AreEqual(newRates["5"], 60.0d);
            Assert.AreEqual(newRates["6"], 30.0d);
        }

        [Test]
        public void DistributionTest2()
        {
            List<FlowConnection> inputConnections = new List<FlowConnection>()
            {
                BuildConnection("1", new FlowConnectionQuota(0), 30),
                BuildConnection("2", new FlowConnectionQuota(0), 20),
                BuildConnection("3", new FlowConnectionQuota(0), 50),
                BuildConnection("3", new FlowConnectionQuota(0), 50),
                BuildConnection("3", new FlowConnectionQuota(0), 50)
            };

            List<FlowConnection> outputConnections = new List<FlowConnection>()
            {
                BuildConnection("4", new FlowConnectionQuota(55.7), 0),
                BuildConnection("5", new FlowConnectionQuota(40), 0),
                BuildConnection("6", new FlowConnectionQuota(4.3), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(newRates["4"], 111.4d);
            Assert.AreEqual(newRates["5"], 80.0d);
            Assert.AreEqual(newRates["6"], 8.6d);
        }

        [Test]
        public void DistributionDirectTest()
        {
            List<FlowConnection> inputConnections = new List<FlowConnection>()
            {
                BuildConnection("1", new FlowConnectionQuota(0), 30),
                BuildConnection("2", new FlowConnectionQuota(0), 20),
                BuildConnection("3", new FlowConnectionQuota(0), 50),
                BuildConnection("3", new FlowConnectionQuota(0), 50),
                BuildConnection("3", new FlowConnectionQuota(0), 50)
            };

            List<FlowConnection> outputConnections = new List<FlowConnection>()
            {
                BuildConnection("4", new FlowConnectionQuota(10, 40), 0),
                BuildConnection("5", new FlowConnectionQuota(20), 0),
                BuildConnection("6", new FlowConnectionQuota(30, 10), 0),
                BuildConnection("7", new FlowConnectionQuota(25), 0),
                BuildConnection("8", new FlowConnectionQuota(25), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(Math.Abs(newRates["4"] - 40.0d) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["5"] - 42.857142) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["6"] - 10.0d) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["7"] - 53.57142) < MaxError, true);
            Assert.AreEqual(Math.Abs(newRates["8"] - 53.57142) < MaxError, true);
        }

        [Test]
        public void DistributionBringingTest()
        {
            List<FlowConnection> inputConnections = new List<FlowConnection>()
            {
                BuildConnection("1", new FlowConnectionQuota(0), 30),
                BuildConnection("2", new FlowConnectionQuota(0), 20)
            };

            List<FlowConnection> outputConnections = new List<FlowConnection>()
            {
                BuildConnection("4", new FlowConnectionQuota(130), 0),
                BuildConnection("5", new FlowConnectionQuota(130), 0)
            };

            var newRates = FlowRateDistributionStrategy.MaxByQuotes(inputConnections, outputConnections);

            Assert.AreEqual(newRates["4"], 25.0d);
            Assert.AreEqual(newRates["5"], 25.0d);
        }
    }
}
