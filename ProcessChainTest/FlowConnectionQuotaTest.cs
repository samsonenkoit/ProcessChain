using NUnit.Framework;
using ProcessChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChainTest
{
    [TestFixture]
    public class FlowConnectionQuotaTest
    {
        [TestCase(typeof(ArgumentOutOfRangeException), -1, 1)]
        [TestCase(typeof(ArgumentOutOfRangeException), 1, -1)]
        public void Constructor_InvalidParameters_Throw(Type exceptionType, double percent, double directValue )
        {
            Assert.Throws(exceptionType, () => { new NodeConnectionQuota(percent, directValue); });
        }

        [Test]
        public void Constructor_OneParameter_Success()
        {
            var quota = new NodeConnectionQuota(12.2d);

            Assert.AreEqual(quota.Percent, 12.2d);
            Assert.AreEqual(quota.DirectValue, NodeConnectionQuota.DirectValueNone);
            Assert.AreEqual(quota.IsDirectValue, false);

            

        }

        [Test]
        public void Constructor_TwoParameters_Success()
        {
            var quota = new NodeConnectionQuota(1.1d, 3.2d);

            Assert.AreEqual(quota.DirectValue, 3.2d);
            Assert.AreEqual(quota.IsDirectValue, true);
        }
    }
}
