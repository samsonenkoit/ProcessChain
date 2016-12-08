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


        [TestCase(typeof(ArgumentOutOfRangeException), "", "1", "2", 12)]
        [TestCase(typeof(ArgumentOutOfRangeException), "1", "", "2", 12)]
        [TestCase(typeof(ArgumentOutOfRangeException), "1", "3", "", 12)]
        public void Constructor_IncorrectParameters_Throw(Type exceptionType, string id, string startNodeId, string endNodeId, double quota)
        {
            Assert.Throws(exceptionType,() => new ConnectionInfo(id, startNodeId, endNodeId, new NodeConnectionQuota(quota)));
        }

        [Test]
        public void Constructor_CorrectParameters_Success()
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
