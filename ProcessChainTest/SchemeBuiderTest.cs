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
    class SchemeBuiderTest
    {
        [Test]
        public void Build_InvalidParameters_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SchemeBuilder.Build(null, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => SchemeBuilder.Build(new List<NodeInfo>(), new List<ConnectionInfo>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => SchemeBuilder.Build(null, new List<ConnectionInfo>()));
        }

        [Test]
        public void BuildAllConnectionsHaveNodeForStartIdValidationTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null)
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2)),
                new ConnectionInfo("c3", "n5", "n3", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildAllConnectionsHaveNodeForEndIdValidationTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null)
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n5", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildAllNodesHaveConnectionValidationTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null),
                new NodeInfo("n5", NodeType.Consumer, null)
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildExtractorsHaveNotInputConnectionsTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null),
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2)),
                new ConnectionInfo("c3", "n2", "n1", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildConsumersHaveNotOutputConnectionsTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null),
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2)),
                new ConnectionInfo("c3", "n3", "n2", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildInstallationsHaveInputConnectionsTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n5", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null),
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2)),
                new ConnectionInfo("c3", "n5", "n3", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildInstallationsHaveOutputConnectionsTest()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n5", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null),
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2)),
                new ConnectionInfo("c3", "n2", "n5", new NodeConnectionQuota(2))
            };

            Assert.Throws<InvalidOperationException>(() => SchemeBuilder.Build(nodes, conns));
        }

        [Test]
        public void BuildTest1()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n3", NodeType.Consumer, null)
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n2", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n2", "n3", new NodeConnectionQuota(2))
            };

            var scheme = SchemeBuilder.Build(nodes, conns);

            Assert.AreEqual(ValidateScheme(nodes, conns, scheme), true);
        }

        [Test]
        public void BuildTest2()
        {
            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Extractor, null),
                new NodeInfo("n3", NodeType.Installation, new NodeScope(10)),
                new NodeInfo("n4", NodeType.Installation, new NodeScope(11)),
                new NodeInfo("n5", NodeType.Installation, new NodeScope(12)),
                new NodeInfo("n6", NodeType.Installation, new NodeScope(13.1)),
                new NodeInfo("n7", NodeType.Consumer, null)
            };
            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("c1", "n1", "n3", new NodeConnectionQuota(1)),
                new ConnectionInfo("c2", "n1", "n4", new NodeConnectionQuota(2)),
                new ConnectionInfo("c3", "n2", "n5", new NodeConnectionQuota(3)),
                new ConnectionInfo("c4", "n2", "n6", new NodeConnectionQuota(4)),

                new ConnectionInfo("c5", "n3", "n7", new NodeConnectionQuota(4)),
                new ConnectionInfo("c6", "n4", "n7", new NodeConnectionQuota(4)),
                new ConnectionInfo("c7", "n5", "n7", new NodeConnectionQuota(4)),
                new ConnectionInfo("c8", "n6", "n7", new NodeConnectionQuota(4)),
            };

            var scheme = SchemeBuilder.Build(nodes, conns);

            Assert.AreEqual(ValidateScheme(nodes, conns, scheme), true);
        }

        private bool ValidateScheme(List<NodeInfo> nodes, List<ConnectionInfo> conns, Scheme scheme)
        {
            var extractors = nodes.Where(t => t.Type == NodeType.Extractor);
            var installations = nodes.Where(t => t.Type == NodeType.Installation);
            var connsumers = nodes.Where(t => t.Type == NodeType.Consumer);

            foreach (var ext in extractors)
            {
                if (!(scheme.Extractors[ext.Id].Id == ext.Id))
                    return false;
            }

            foreach (var ins in installations)
            {
                if (!(scheme.Installations[ins.Id].Id == ins.Id) &&
                    (scheme.Installations[ins.Id].Scope.RateValueMax == ins.Scope.RateValueMax))
                    return false;
            }

            foreach (var cons in connsumers)
            {
                if (!(scheme.Consumers[cons.Id].Id == cons.Id))
                    return false;
            }

            foreach (var conn in conns)
            {
                var flConn = scheme.Connections[conn.Id];

                if (flConn.Input.Id != conn.StartNodeId || flConn.Output.Id != conn.EndNodeId ||
                    flConn.Quota.Percent != conn.Quota.Percent || flConn.Quota.DirectValue != conn.Quota.DirectValue)
                    return false;
            }

            return true;
        }
    }
}
