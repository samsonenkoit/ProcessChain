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
        [Test]
        public void ConstructorValidationTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new FlowConnectionQuota(-1, 1); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new FlowConnectionQuota(1, -1); });
        }

        [Test]
        public void ConstructorTest()
        {
            var quota = new FlowConnectionQuota(12.2d);

            Assert.AreEqual(quota.Percent, 12.2d);
            Assert.AreEqual(quota.DirectValue, FlowConnectionQuota.DirectValueNone);
            Assert.AreEqual(quota.IsDirectValue, false);

            quota = new FlowConnectionQuota(1.1d, 3.2d);

            Assert.AreEqual(quota.DirectValue, 3.2d);
            Assert.AreEqual(quota.IsDirectValue, true);

        } 
    }
}
