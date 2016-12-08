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
        private Extractor GetDefaulteExtractor()
        {
            return new Extractor("1");
        }

        [Test]
        public void SetOutputConnection_NullParameter_Throw()
        {
            Extractor ext = GetDefaulteExtractor();

            Assert.Throws<ArgumentNullException>(() => { ext.SetOutputConnection(null); });
        }

        [Test]
        public void SetOutputConnection_SecondSet_Throw()
        {
            Extractor ext = GetDefaulteExtractor();

            FakeFlowElement flowEl = new FakeFlowElement();
            NodeConnection conn = new NodeConnection("c1", new NodeConnectionQuota(0), ext, flowEl);

            ext.SetOutputConnection(conn);
            Assert.Throws<InvalidOperationException>(() => ext.SetOutputConnection(conn));
        }

        [Test]
        public void UpdateFlowRateParameter_InvalidParamater_Throw()
        {
            Extractor ext = GetDefaulteExtractor();

            Assert.Throws<ArgumentOutOfRangeException>(() => ext.FlowRateUpdate(-1));
        }

        [TestCase(10.1d)]
        [TestCase(1.2d)]
        [TestCase(5.3d)]
        public void FlowRateUpdate_UpdateOutputConnectionFlowRate_Success(double newFlowRate)
        {
            Extractor ext = GetDefaulteExtractor();
            FakeFlowElement stubFlowEl = new FakeFlowElement("2");

            NodeConnection mockConn = new NodeConnection("c1", new NodeConnectionQuota(1), ext, stubFlowEl);
            ext.SetOutputConnection(mockConn);
            stubFlowEl.Connection = mockConn;

            var result = ext.FlowRateUpdate(newFlowRate);
            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(ext.FlowRate, newFlowRate);
            Assert.AreEqual(stubFlowEl.InputFlowRate["c1"], newFlowRate);
        }
    }
}
