using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Builder
{
    public class ConnectionInfo
    {
        public string Id { get; set; }
        public string StartNodeId { get; private set; }
        public string EndNodeId { get; private set; }
        public NodeConnectionQuota Quota { get; private set; }

        public ConnectionInfo(string id, string startNodeId, string endNodeId, NodeConnectionQuota quota)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentOutOfRangeException(nameof(id)); 
            if (string.IsNullOrEmpty(startNodeId)) throw new ArgumentOutOfRangeException(nameof(startNodeId));
            if (string.IsNullOrEmpty(endNodeId)) throw new ArgumentOutOfRangeException(nameof(endNodeId));
            if (quota == null) throw new ArgumentNullException(nameof(quota));

            Id = id;
            StartNodeId = startNodeId;
            EndNodeId = endNodeId;
            Quota = quota;
        }

    }
}
