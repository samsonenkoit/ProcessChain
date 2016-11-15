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
    public class FlowRateUpdateResultTest
    {
        [Test]
        public void ConstructorTest()
        {
            var result = new FlowRateUpdateResult();

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(result.Restriction, null);

            var restriction = new FlowRestriction(FlowRestrictionTypes.FlowRateValueMaximum, new FlowElementMock(), 1);

            result = new FlowRateUpdateResult(restriction);

            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction, restriction);
        }
    }
}
