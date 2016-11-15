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
    public class FlowRestrictionTest
    {
        [Test]
        public void ConstructorValidationTest()
        {
            Assert.Throws<ArgumentNullException>(() => { new FlowRestriction(FlowRestrictionTypes.FlowRateValueMaximum, null, 12); });
        }

        [Test]
        public void ConstructorTest()
        {
            FlowElementMock flM = new FlowElementMock();

            FlowRestriction rest = new FlowRestriction(FlowRestrictionTypes.InputFlowNotEqualOutput, flM, 15.1d);

            Assert.AreEqual(rest.Type, FlowRestrictionTypes.InputFlowNotEqualOutput);
            Assert.AreEqual(rest.Element, flM);
            Assert.AreEqual(rest.RestrictionValue, 15.1d);
        }
    }
}
