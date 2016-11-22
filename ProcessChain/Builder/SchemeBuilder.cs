using ProcessChain.Extension;
using ProcessChain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Builder
{
    public static class SchemeBuilder
    {
        public static Scheme Build(IList<NodeInfo> nodes, IList<ConnectionInfo> connections)
        {
            ValidateModel(nodes, connections);

            List<Extractor> extractors = nodes.Where(t => t.Type == NodeType.Extractor).Select(t => new Extractor(t.Id)).ToList();
            List<Consumer> consumers = nodes.Where(t => t.Type == NodeType.Consumer).Select(t => new Consumer(t.Id)).ToList();
            List<InputsOutputsElement> elements = nodes.Where(t => t.Type == NodeType.Installation)
                .Select(t => new InputsOutputsElement(t.Id, t.Scope, FlowRateDistributionStrategy.MaxByQuotes)).ToList();

            Dictionary<string, NodeElement> allElements = new Dictionary<string, NodeElement>();
            allElements.AddRange(extractors.Select(t => new KeyValuePair<string, NodeElement>(t.Id, t)));
            allElements.AddRange(consumers.Select(t => new KeyValuePair<string, NodeElement>(t.Id, t)));
            allElements.AddRange(elements.Select(t => new KeyValuePair<string, NodeElement>(t.Id, t)));

            List<NodeConnection> conns = connections.Select(t => new NodeConnection(t.Id, t.Quota,
                allElements[t.StartNodeId], allElements[t.EndNodeId])).ToList();

            foreach(var ext in extractors)
            {
                ext.SetOutputConnection(conns.First(t => t.Input == ext));
            }

            foreach(var cons in consumers)
            {
                cons.SetInputConnections(conns.Where(t => t.Output == cons).ToList());
            }

            foreach(var elem in elements)
            {
                elem.SetConnections(conns.Where(t => t.Output == elem).ToList(),
                    conns.Where(t => t.Input == elem).ToList());
            }

            Dictionary<string, Extractor> extDict = new Dictionary<string, Extractor>();
            extDict.AddRange(extractors.Select(t => new KeyValuePair<string, Extractor>(t.Id, t)).ToList());
            Dictionary<string, Consumer> consDict = new Dictionary<string, Consumer>();
            consDict.AddRange(consumers.Select(t => new KeyValuePair<string, Consumer>(t.Id, t)));
            Dictionary<string, InputsOutputsElement> elemsDict = new Dictionary<string, InputsOutputsElement>();
            elemsDict.AddRange(elements.Select(t => new KeyValuePair<string, InputsOutputsElement>(t.Id, t)));
            Dictionary<string, NodeConnection> connsDict = new Dictionary<string, NodeConnection>();
            connsDict.AddRange(conns.Select(t => new KeyValuePair<string, NodeConnection>(t.Id, t)));

            return new Scheme(extDict, elemsDict, consDict, connsDict);
            
        }

        #region Validation

        private static void ValidateModel(IList<NodeInfo> nodes, IList<ConnectionInfo> connections)
        {
            if (nodes == null || !nodes.Any()) throw new ArgumentOutOfRangeException(nameof(nodes));
            if (connections == null || !nodes.Any()) throw new ArgumentOutOfRangeException(nameof(connections));

            ValidateBaseConnections(nodes, connections);
            ValidateExtractorConnections(nodes, connections);
            ValidateConsumerConnections(nodes, connections);
            ValidateInstallationsConnections(nodes, connections);
        }

        /// <summary>
        /// Проверяет что для всех соединений есть узлы, а для всех узлов - соединения
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="connections"></param>
        private static void ValidateBaseConnections(IList<NodeInfo> nodes, IList<ConnectionInfo> connections)
        {
            //проверяем что для всех соединений есть узлы
            var invalidConn = connections.FirstOrDefault(t => !nodes.Any(g => g.Id == t.StartNodeId) ||
                !nodes.Any(g => g.Id == t.EndNodeId));

            if (invalidConn != null)
                throw new InvalidOperationException($"Connection {invalidConn.Id} have't nodes");

            //проверяем что для всех узлов есть соединения
            var invalidNode = nodes.FirstOrDefault(t => !connections.Any(g => g.StartNodeId == t.Id || g.EndNodeId == t.Id));

            if (invalidNode != null)
                throw new InvalidOperationException($"Node {invalidNode.Id} have't connection");
        }

        /// <summary>
        /// Проверяет, что для генераторов нет входных соединений
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="connections"></param>
        private static void ValidateExtractorConnections(IList<NodeInfo> nodes, IList<ConnectionInfo> connections)
        {
            var invalidExtractor = nodes.FirstOrDefault(t => t.Type == NodeType.Extractor &&
                connections.Any(g => g.EndNodeId == t.Id));

            if (invalidExtractor != null)
                throw new InvalidOperationException($"Extractor {invalidExtractor.Id}. Extractors musn't have input connections");
        }

        /// <summary>
        /// Проверяет что для хранилищ нет выходных соединений
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="connections"></param>
        private static void ValidateConsumerConnections(IList<NodeInfo> nodes, IList<ConnectionInfo> connections)
        {
            var invalidConsumer = nodes.FirstOrDefault(t => t.Type == NodeType.Consumer && connections.Any(g => g.StartNodeId == t.Id));

            if (invalidConsumer != null)
                throw new InvalidOperationException($"Consumer {invalidConsumer.Id}. Consumer musn't have output connections");
        }
        
        private static void ValidateInstallationsConnections(IList<NodeInfo> nodes, IList<ConnectionInfo> connections)
        {
            var invalidInstallation = nodes.FirstOrDefault(t => t.Type == NodeType.Installation &&
                (!connections.Any(g => g.StartNodeId == t.Id) || !connections.Any(g => g.EndNodeId == t.Id)));

            if (invalidInstallation != null)
                throw new InvalidOperationException($"Installation {invalidInstallation.Id} must have input and output connections");
        }

        #endregion
    }
}
