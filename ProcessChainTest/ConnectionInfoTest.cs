using NUnit.Framework;
using ProcessChain;
using ProcessChain.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChainTest
{
    [TestFixture]
    public class ConnectionInfoTest
    {
        [Test]
        public void ConstructorParameterValidationTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConnectionInfo("", "1", "2", new NodeConnectionQuota(12)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConnectionInfo("1", "", "2", new NodeConnectionQuota(12)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConnectionInfo("1", "3", "", new NodeConnectionQuota(12)));
            Assert.Throws<ArgumentNullException>(() => new ConnectionInfo("1", "3", "4", null));
        }

        [Test]
        public void ConstructorTest()
        {
            var quota = new NodeConnectionQuota(12);
            var connInfo = new ConnectionInfo("1", "2", "3", quota);

            Assert.AreEqual(connInfo.Id, "1");
            Assert.AreEqual(connInfo.StartNodeId, "2");
            Assert.AreEqual(connInfo.EndNodeId, "3");
            Assert.AreEqual(connInfo.Quota, quota);
        }
    }
}
