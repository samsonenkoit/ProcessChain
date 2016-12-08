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
    public class InputsOutputsElementTest
    {
        [Test]
        public void Constructor_ValidParameters_Throw()
        {
            Assert.DoesNotThrow(delegate
            {
                new InputsOutputsElement("1", new NodeScope(1),
                    FlowRateDistributionStrategy.MaxByQuotes);
            });
        }

        [Test]
        public void Constructor_InvalidParameters_Throw()
        {
            Assert.Throws<ArgumentNullException>(delegate
            {
                new InputsOutputsElement("1", null,
                    FlowRateDistributionStrategy.MaxByQuotes);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                new InputsOutputsElement("1", new NodeScope(12), null);
            });
        }

        [Test]
        public void SetConnections_SecondParameterSet_Throw()
        {
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(1),
                FlowRateDistributionStrategy.MaxByQuotes);

            el.SetConnections(new List<NodeConnection>(), new List<NodeConnection>());
            Assert.Throws<InvalidOperationException>(() => { el.SetConnections(new List<NodeConnection>(), new List<NodeConnection>()); });
        }

        [Test]
        public void SetConnections_InvalidParameters_Throw()
        {
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(1),
                FlowRateDistributionStrategy.MaxByQuotes);

            Assert.Throws<ArgumentNullException>(() => { el.SetConnections(new List<NodeConnection>(), null); });
            Assert.Throws<ArgumentNullException>(() => { el.SetConnections(null, new List<NodeConnection>()); });
           
        }

        [Test]
        public void UpdateFlowRates_IncorrectSchema_DetectRestriction()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(17),
                FlowRateDistributionStrategy.MaxByQuotes);
            FakeFlowElement elM1 = new FakeFlowElement();
            FakeFlowElement elM2 = new FakeFlowElement();
            FakeFlowElement elM3 = new FakeFlowElement();
            FakeFlowElement elM4 = new FakeFlowElement();

            var inputConn = new List<NodeConnection>()
            {
                new NodeConnection("c1", new NodeConnectionQuota(50), elM1, el, 10),
                new NodeConnection("c2", new NodeConnectionQuota(50), elM2, el, 20)
            };

            var outputConn = new List<NodeConnection>()
            {
                new NodeConnection("c3", new NodeConnectionQuota(20, 13), el, elM3, 30),
                new NodeConnection("c4", new NodeConnectionQuota(80), el, elM4, 30)
            };

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.FlowRateValueMaximum);
            Assert.AreEqual(result.Restriction.RestrictionValue, 13);
        }

        [Test]
        public void UpdateFlowRates_IncorrectSchema_DetectRestriction2()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(40),
                FlowRateDistributionStrategy.MaxByQuotes);
            FakeFlowElement elM1 = new FakeFlowElement();
            FakeFlowElement elM2 = new FakeFlowElement();
            FakeFlowElement elM3 = new FakeFlowElement();
            FakeFlowElement elM4 = new FakeFlowElement();

            var inputConn = new List<NodeConnection>()
            {
                new NodeConnection("c1", new NodeConnectionQuota(50), elM1, el, 10),
                new NodeConnection("c2", new NodeConnectionQuota(50), elM2, el, 10)
            };

            var outputConn = new List<NodeConnection>()
            {
                new NodeConnection("c3", new NodeConnectionQuota(20, 22), el, elM3, 30),
                new NodeConnection("c4", new NodeConnectionQuota(80), el, elM4, 30)
            };

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.InputFlowNotEqualOutput);
            Assert.AreEqual(result.Restriction.RestrictionValue, 2);
        }

        [Test]
        public void UpdateFlowRates_CorrectSchema_Success()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(300),
                FlowRateDistributionStrategy.MaxByQuotes);
            FakeFlowElement elM1 = new FakeFlowElement();
            FakeFlowElement elM2 = new FakeFlowElement();
            FakeFlowElement elM3 = new FakeFlowElement();
            FakeFlowElement elM4 = new FakeFlowElement();
            FakeFlowElement elM5 = new FakeFlowElement();

            var inputConn = new List<NodeConnection>()
            {
                new NodeConnection("c1", new NodeConnectionQuota(50), elM1, el, 50),
                new NodeConnection("c2", new NodeConnectionQuota(50), elM2, el, 100)
            };

            var outputConn = new List<NodeConnection>()
            {
                new NodeConnection("c3", new NodeConnectionQuota(20, 50), el, elM3, 30),
                new NodeConnection("c4", new NodeConnectionQuota(19.6), el, elM4, 30),
                new NodeConnection("c5", new NodeConnectionQuota(80.4), el, elM5, 30)
            };

            elM3.Connection = outputConn[0];
            elM4.Connection = outputConn[1];
            elM5.Connection = outputConn[2];

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();
            Assert.AreEqual(result.IsSuccess, true);

            Assert.AreEqual(outputConn[0].CurrentRate, 50);
            Assert.AreEqual(outputConn[1].CurrentRate, 19.6d);
            Assert.AreEqual(outputConn[2].CurrentRate, 80.4d);

            Assert.AreEqual(elM3.InputFlowRate["c3"], 50);
            Assert.AreEqual(elM4.InputFlowRate["c4"], 19.6d);
            Assert.AreEqual(elM5.InputFlowRate["c5"], 80.4d);
        }

        [Test]
        public void UpdateOutputConnectionsQuota_IncorrectParameters_Throw()
        {
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(300),
                FlowRateDistributionStrategy.MaxByQuotes);

            Assert.Throws<ArgumentNullException>(() => el.UpdateOutputConnectionsQuota(null));

        }

        [Test]
        public void UpdateOutputConnectionsQuota_ValidParameters_Success()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new NodeScope(300),
                FlowRateDistributionStrategy.MaxByQuotes);
            FakeFlowElement elM1 = new FakeFlowElement();
            FakeFlowElement elM2 = new FakeFlowElement();
            FakeFlowElement elM3 = new FakeFlowElement();
            FakeFlowElement elM4 = new FakeFlowElement();
            FakeFlowElement elM5 = new FakeFlowElement();

            var inputConn = new List<NodeConnection>()
            {
                new NodeConnection("c1", new NodeConnectionQuota(50), elM1, el, 20),
                new NodeConnection("c2", new NodeConnectionQuota(50), elM2, el, 80)
            };

            var outputConn = new List<NodeConnection>()
            {
                new NodeConnection("c3", new NodeConnectionQuota(20, 50), el, elM3, 30),
                new NodeConnection("c4", new NodeConnectionQuota(21.1), el, elM4, 30),
                new NodeConnection("c5", new NodeConnectionQuota(78.9), el, elM5, 30)
            };

            elM3.Connection = outputConn[0];
            elM4.Connection = outputConn[1];
            elM5.Connection = outputConn[2];

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();

            Dictionary<string, NodeConnectionQuota> newQutas = new Dictionary<string, NodeConnectionQuota>();
            newQutas.Add("c3", new NodeConnectionQuota(10));
            newQutas.Add("c4", new NodeConnectionQuota(30));
            newQutas.Add("c5", new NodeConnectionQuota(60, 60));

            result = el.UpdateOutputConnectionsQuota(newQutas);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(elM3.InputFlowRate["c3"], 10);
            Assert.AreEqual(elM4.InputFlowRate["c4"], 30);
            Assert.AreEqual(elM5.InputFlowRate["c5"], 60);
        }
    }
}
