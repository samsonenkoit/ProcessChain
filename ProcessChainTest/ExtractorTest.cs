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
    public class ExtractorTest
    {
        [Test]
        public void SetOutputConnectionTest()
        {
            Extractor ext = new Extractor("1");

            Assert.Throws<ArgumentNullException>(() => { ext.SetOutputConnection(null); });

            FlowElementMock flowEl = new FlowElementMock();
            FlowConnection conn = new FlowConnection("c1", new FlowConnectionQuota(0), ext, flowEl);

            Assert.DoesNotThrow(() => ext.SetOutputConnection(conn));
            Assert.Throws<InvalidOperationException>(() => ext.SetOutputConnection(conn));
        }

        [Test]
        public void UpdateFlowRateParameterValidationTest()
        {
            Extractor ext = new Extractor("1");

            Assert.Throws<ArgumentOutOfRangeException>(() => ext.FlowRateUpdate(-1));
        }

        [Test]
        public void UpdateFlowRateTest()
        {
            Extractor ext = new Extractor("1");
            FlowElementMock flEl = new FlowElementMock("2");

            FlowConnection conn = new FlowConnection("c1", new FlowConnectionQuota(1), ext, flEl);
            ext.SetOutputConnection(conn);
            flEl.Connection = conn;

            var result = ext.FlowRateUpdate(10.1d);
            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(ext.FlowRate, 10.1d);
            Assert.AreEqual(flEl.InputFlowRate["c1"], 10.1d);

            ext.FlowRateUpdate(1.2d);
            Assert.AreEqual(flEl.InputFlowRate["c1"], 1.2d);
        }
    }
}
