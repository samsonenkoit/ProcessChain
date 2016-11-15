using ProcessChain.Extension;
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
            if (nodes == null || !nodes.Any()) throw new ArgumentOutOfRangeException(nameof(nodes));
            if (connections == null || !nodes.Any()) throw new ArgumentOutOfRangeException(nameof(connections));

            List<Extractor> extractors = nodes.Where(t => t.Type == NodeType.Extractor).Select(t => new Extractor(t.Id)).ToList();
            List<Consumer> consumers = nodes.Where(t => t.Type == NodeType.Consumer).Select(t => new Consumer(t.Id)).ToList();
            List<InputsOutputsElement> elements = nodes.Where(t => t.Type == NodeType.Installation)
                .Select(t => new InputsOutputsElement(t.Id, t.Scope, FlowRateDistributionStrategy.MaxByQuotes)).ToList();

            Dictionary<string, FlowElement> allElements = new Dictionary<string, FlowElement>();
            allElements.AddRange(extractors.Select(t => new KeyValuePair<string, FlowElement>(t.Id, t)));
            allElements.AddRange(consumers.Select(t => new KeyValuePair<string, FlowElement>(t.Id, t)));
            allElements.AddRange(elements.Select(t => new KeyValuePair<string, FlowElement>(t.Id, t)));

            List<FlowConnection> conns = connections.Select(t => new FlowConnection(t.Id, t.Quota,
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
            Dictionary<string, FlowConnection> connsDict = new Dictionary<string, FlowConnection>();
            connsDict.AddRange(conns.Select(t => new KeyValuePair<string, FlowConnection>(t.Id, t)));

            return new Scheme(extDict, elemsDict, consDict, connsDict);
            
        }
    }
}
