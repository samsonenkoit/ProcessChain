﻿using NUnit.Framework;
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
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(delegate
            {
                new InputsOutputsElement("1", new FlowElementScope(1),
                    FlowRateDistributionStrategy.MaxByQuotes);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                new InputsOutputsElement("1", null,
                    FlowRateDistributionStrategy.MaxByQuotes);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                new InputsOutputsElement("1", new FlowElementScope(12), null);
            });
        }

        [Test]
        public void SetConnectionsTest()
        {
            InputsOutputsElement el = new InputsOutputsElement("1", new FlowElementScope(1),
                FlowRateDistributionStrategy.MaxByQuotes);

            Assert.Throws<ArgumentNullException>(() => { el.SetConnections(new List<FlowConnection>(), null); });
            Assert.Throws<ArgumentNullException>(() => { el.SetConnections(null, new List<FlowConnection>()); });
            Assert.DoesNotThrow(() => { el.SetConnections(new List<FlowConnection>(), new List<FlowConnection>()); });
            Assert.Throws<InvalidOperationException>(() => { el.SetConnections(new List<FlowConnection>(), new List<FlowConnection>()); });
        }

        [Test]
        public void UpdateFlowRatesValidationTest1()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new FlowElementScope(17),
                FlowRateDistributionStrategy.MaxByQuotes);
            FlowElementMock elM1 = new FlowElementMock();
            FlowElementMock elM2 = new FlowElementMock();
            FlowElementMock elM3 = new FlowElementMock();
            FlowElementMock elM4 = new FlowElementMock();

            var inputConn = new List<FlowConnection>()
            {
                new FlowConnection("c1", new FlowConnectionQuota(50), elM1, el, 10),
                new FlowConnection("c2", new FlowConnectionQuota(50), elM2, el, 20)
            };

            var outputConn = new List<FlowConnection>()
            {
                new FlowConnection("c3", new FlowConnectionQuota(20, 13), el, elM3, 30),
                new FlowConnection("c4", new FlowConnectionQuota(80), el, elM4, 30)
            };

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.FlowRateValueMaximum);
            Assert.AreEqual(result.Restriction.RestrictionValue, 13);
        }

        [Test]
        public void UpdateFlowRatesValidationTest2()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new FlowElementScope(40),
                FlowRateDistributionStrategy.MaxByQuotes);
            FlowElementMock elM1 = new FlowElementMock();
            FlowElementMock elM2 = new FlowElementMock();
            FlowElementMock elM3 = new FlowElementMock();
            FlowElementMock elM4 = new FlowElementMock();

            var inputConn = new List<FlowConnection>()
            {
                new FlowConnection("c1", new FlowConnectionQuota(50), elM1, el, 10),
                new FlowConnection("c2", new FlowConnectionQuota(50), elM2, el, 10)
            };

            var outputConn = new List<FlowConnection>()
            {
                new FlowConnection("c3", new FlowConnectionQuota(20, 22), el, elM3, 30),
                new FlowConnection("c4", new FlowConnectionQuota(80), el, elM4, 30)
            };

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.InputFlowNotEqualOutput);
            Assert.AreEqual(result.Restriction.RestrictionValue, 2);
        }

        [Test]
        public void UpdateFlowRatesTest()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new FlowElementScope(300),
                FlowRateDistributionStrategy.MaxByQuotes);
            FlowElementMock elM1 = new FlowElementMock();
            FlowElementMock elM2 = new FlowElementMock();
            FlowElementMock elM3 = new FlowElementMock();
            FlowElementMock elM4 = new FlowElementMock();
            FlowElementMock elM5 = new FlowElementMock();

            var inputConn = new List<FlowConnection>()
            {
                new FlowConnection("c1", new FlowConnectionQuota(50), elM1, el, 50),
                new FlowConnection("c2", new FlowConnectionQuota(50), elM2, el, 100)
            };

            var outputConn = new List<FlowConnection>()
            {
                new FlowConnection("c3", new FlowConnectionQuota(20, 50), el, elM3, 30),
                new FlowConnection("c4", new FlowConnectionQuota(19.6), el, elM4, 30),
                new FlowConnection("c5", new FlowConnectionQuota(80.4), el, elM5, 30)
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
        public void UpdateOutputConnectionsQuotaValidationTest()
        {
            InputsOutputsElement el = new InputsOutputsElement("1", new FlowElementScope(300),
                FlowRateDistributionStrategy.MaxByQuotes);

            Assert.Throws<ArgumentNullException>(() => el.UpdateOutputConnectionsQuota(null));

        }

        [Test]
        public void UpdateOutputConnectionsQuota()
        {
            #region Test data
            InputsOutputsElement el = new InputsOutputsElement("1", new FlowElementScope(300),
                FlowRateDistributionStrategy.MaxByQuotes);
            FlowElementMock elM1 = new FlowElementMock();
            FlowElementMock elM2 = new FlowElementMock();
            FlowElementMock elM3 = new FlowElementMock();
            FlowElementMock elM4 = new FlowElementMock();
            FlowElementMock elM5 = new FlowElementMock();

            var inputConn = new List<FlowConnection>()
            {
                new FlowConnection("c1", new FlowConnectionQuota(50), elM1, el, 20),
                new FlowConnection("c2", new FlowConnectionQuota(50), elM2, el, 80)
            };

            var outputConn = new List<FlowConnection>()
            {
                new FlowConnection("c3", new FlowConnectionQuota(20, 50), el, elM3, 30),
                new FlowConnection("c4", new FlowConnectionQuota(21.1), el, elM4, 30),
                new FlowConnection("c5", new FlowConnectionQuota(78.9), el, elM5, 30)
            };

            elM3.Connection = outputConn[0];
            elM4.Connection = outputConn[1];
            elM5.Connection = outputConn[2];

            el.SetConnections(inputConn, outputConn);

            #endregion

            var result = el.UpdateFlowRates();

            Dictionary<string, FlowConnectionQuota> newQutas = new Dictionary<string, FlowConnectionQuota>();
            newQutas.Add("c3", new FlowConnectionQuota(10));
            newQutas.Add("c4", new FlowConnectionQuota(30));
            newQutas.Add("c5", new FlowConnectionQuota(60, 60));

            result = el.UpdateOutputConnectionsQuota(newQutas);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(elM3.InputFlowRate["c3"], 10);
            Assert.AreEqual(elM4.InputFlowRate["c4"], 30);
            Assert.AreEqual(elM5.InputFlowRate["c5"], 60);
        }
    }
}