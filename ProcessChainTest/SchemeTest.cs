﻿using NUnit.Framework;
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
    class SchemeTest
    {
        #region Static

        private static double MaxError = 0.00001d;

        #endregion

        [Test]
        public void ConstructorValidationTest()
        {
            Assert.Throws<ArgumentNullException>(() => new Scheme(null, null, null, null));
        }

        [Test]
        public void ConstructorTest()
        {
            var ext = new Dictionary<string, Extractor>();
            var inst = new Dictionary<string, InputsOutputsElement>();
            var cons = new Dictionary<string, Consumer>();
            var conns = new Dictionary<string, NodeConnection>();

            var scheme = new Scheme(ext, inst, cons, conns);

            Assert.AreEqual(scheme.Extractors, ext);
            Assert.AreEqual(scheme.Installations, inst);
            Assert.AreEqual(scheme.Consumers, cons);
            Assert.AreEqual(scheme.Connections, conns);
        }

        [Test]
        public void Scheme1WorkTest()
        {
            #region Data
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
            #endregion

            var scheme = SchemeBuilder.Build(nodes, conns);

            var result = scheme.Extractors["n1"].FlowRateUpdate(1.1d);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Extractors["n1"].FlowRate, 1.1d);
            Assert.AreEqual(scheme.Installations["n2"].FlowRate, 1.1d);
            Assert.AreEqual(scheme.Consumers["n3"].FlowRate, 1.1d);

            var newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("c2", new NodeConnectionQuota(10, 5));
            result = scheme.Installations["n2"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Element.Id, "n2");
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.InputFlowNotEqualOutput);
            Assert.AreEqual(result.Restriction.RestrictionValue, 3.9d);

            result = scheme.Extractors["n1"].FlowRateUpdate(200);
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Element.Id, "n2");
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.FlowRateValueMaximum);
            Assert.AreEqual(result.Restriction.RestrictionValue, 190);

            result = scheme.Extractors["n1"].FlowRateUpdate(5);
            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Extractors["n1"].FlowRate, 5d);
            Assert.AreEqual(scheme.Installations["n2"].FlowRate, 5d);
            Assert.AreEqual(scheme.Consumers["n3"].FlowRate, 5d);
        }

        [Test]
        public void Scheme2WorkTest()
        {
            #region Data

            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n10", NodeType.Extractor, null),
                new NodeInfo("n11", NodeType.Extractor, null),

                new NodeInfo("n1", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n3", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n4", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n5", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n6", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n7", NodeType.Installation, new NodeScope(200)),
                new NodeInfo("n8", NodeType.Installation, new NodeScope(200)),

                new NodeInfo("n9", NodeType.Consumer, null)
            };

            List<ConnectionInfo> conns = new List<ConnectionInfo>()
            {
                new ConnectionInfo("n10n1", "n10", "n1", new NodeConnectionQuota(100)),
                new ConnectionInfo("n11n2", "n11", "n2", new NodeConnectionQuota(100)),

                new ConnectionInfo("n1n3", "n1", "n3", new NodeConnectionQuota(20)),
                new ConnectionInfo("n1n4", "n1", "n4", new NodeConnectionQuota(80)),
                new ConnectionInfo("n2n5", "n2", "n5", new NodeConnectionQuota(30)),
                new ConnectionInfo("n2n6", "n2", "n6", new NodeConnectionQuota(70)),
                new ConnectionInfo("n3n7", "n3", "n7", new NodeConnectionQuota(40)),
                new ConnectionInfo("n4n7", "n4", "n7", new NodeConnectionQuota(60)),
                new ConnectionInfo("n5n7", "n5", "n7", new NodeConnectionQuota(30)),
                new ConnectionInfo("n5n9", "n5", "n9", new NodeConnectionQuota(40)),
                new ConnectionInfo("n5n8", "n5", "n8", new NodeConnectionQuota(30)),
                new ConnectionInfo("n6n7", "n6", "n7", new NodeConnectionQuota(10)),
                new ConnectionInfo("n6n8", "n6", "n8", new NodeConnectionQuota(90)),
                new ConnectionInfo("n7n9", "n7", "n9", new NodeConnectionQuota(100)),
                new ConnectionInfo("n8n9", "n8", "n9", new NodeConnectionQuota(100)),
            };

            #endregion

            var scheme = SchemeBuilder.Build(nodes, conns);

            var result = scheme.Extractors["n10"].FlowRateUpdate(100);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Consumers["n9"].FlowRate, 100);
            Assert.AreEqual(scheme.Installations["n3"].FlowRate, 20);
            Assert.AreEqual(scheme.Installations["n4"].FlowRate, 80);
            Assert.AreEqual(scheme.Installations["n7"].FlowRate, 100);
            Assert.AreEqual(scheme.Installations["n6"].FlowRate, 0);

            result = scheme.Extractors["n11"].FlowRateUpdate(200);
            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Installations["n6"].FlowRate, 140);
            Assert.AreEqual(scheme.Installations["n7"].FlowRate, 132);
            Assert.AreEqual(scheme.Consumers["n9"].FlowRate, 300);

            var newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n5n7", new NodeConnectionQuota(40));
            newQuota.Add("n5n9", new NodeConnectionQuota(40));
            newQuota.Add("n5n8", new NodeConnectionQuota(20));
            result = scheme.Installations["n5"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Installations["n7"].FlowRate, 138);

            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n7n9", new NodeConnectionQuota(100, 139));
            result = scheme.Installations["n7"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.RestrictionValue, 1);
            Assert.AreEqual(result.Restriction.Element.Id, "n7");

            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n7n9", new NodeConnectionQuota(100, NodeConnectionQuota.DirectValueNone));
            result = scheme.Installations["n7"].UpdateOutputConnectionsQuota(newQuota);

            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n5n9", new NodeConnectionQuota(100, 40));
            result = scheme.Installations["n5"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(scheme.Installations["n8"].FlowRate - 132.666666666 < MaxError, true);

        }

        [Test]
        public void Scheme3WorkTest()
        {
            #region Data

            List<NodeInfo> nodes = new List<NodeInfo>()
            {
                new NodeInfo("n1", NodeType.Extractor, null),
                new NodeInfo("n2", NodeType.Installation, new NodeScope(100)),
                new NodeInfo("n3", NodeType.Consumer, null),
                new NodeInfo("n4", NodeType.Consumer, null)
            };

            List<ConnectionInfo> connections = new List<ConnectionInfo>()
            {
                new ConnectionInfo("n1n2", "n1", "n2", new NodeConnectionQuota(100)),
                new ConnectionInfo("n2n3", "n2", "n3", new NodeConnectionQuota(20)),
                new ConnectionInfo("n2n4", "n2", "n4", new NodeConnectionQuota(80))
            };

            var scheme = SchemeBuilder.Build(nodes, connections);

            #endregion

            #region Step 1

            var result = scheme.Extractors["n1"].FlowRateUpdate(105);

            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.RestrictionValue, 5);

            #endregion

            #region Step 2 

            result = scheme.Extractors["n1"].FlowRateUpdate(50);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Installations["n2"].FlowRate, 50);
            Assert.AreEqual(scheme.Consumers["n3"].FlowRate, 10);
            Assert.AreEqual(scheme.Consumers["n4"].FlowRate, 40);

            #endregion

            #region Step 3

            var newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n2n3", new NodeConnectionQuota(50));
            newQuota.Add("n2n4", new NodeConnectionQuota(50));

            result = scheme.Installations["n2"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Consumers["n4"].FlowRate, 25);

            #endregion

            #region Step 4

            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n2n3", new NodeConnectionQuota(100));
            newQuota.Add("n2n4", new NodeConnectionQuota(0));

            result = scheme.Installations["n2"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Consumers["n3"].FlowRate, 50);
            Assert.AreEqual(scheme.Consumers["n4"].FlowRate, 0);

            #endregion

            #region Step 5

            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n2n4", new NodeConnectionQuota(0, 1));

            result = scheme.Installations["n2"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(scheme.Consumers["n3"].FlowRate, 49);
            Assert.AreEqual(scheme.Consumers["n4"].FlowRate, 1);

            #endregion

            #region Step 6

            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n2n3", new NodeConnectionQuota(100, 51));

            result = scheme.Installations["n2"].UpdateOutputConnectionsQuota(newQuota);

            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Restriction.Type, FlowRestrictionTypes.InputFlowNotEqualOutput);
            Assert.AreEqual(result.Restriction.RestrictionValue, 2);
             
            #endregion
        }
    }
}
