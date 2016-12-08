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
    public class NodeInfoTest
    {
        [Test]
        public void Constructor_InvalidParameters_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new NodeInfo("", NodeType.Consumer, new NodeScope(1)));

            
        }

        [Test]
        public void Constructor_ValidParameters_Success()
        {
            var scope = new NodeScope(12);

            var nodeInfo = new NodeInfo("21", NodeType.Extractor, scope);

            Assert.AreEqual(nodeInfo.Id, "21");
            Assert.AreEqual(nodeInfo.Type, NodeType.Extractor);
            Assert.AreEqual(nodeInfo.Scope, scope);
        }
    }
}
