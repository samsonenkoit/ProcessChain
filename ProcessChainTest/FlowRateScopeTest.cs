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
    public class FlowRateScopeTest
    {
        [Test]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new NodeScope(-1); });

            NodeScope scope = new NodeScope(12.0d);
            Assert.AreEqual(scope.RateValueMax, 12.0d);
        }
    }
}
