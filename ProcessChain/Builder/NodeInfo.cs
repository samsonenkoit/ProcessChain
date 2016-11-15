using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Builder
{
    public enum NodeType
    {
        /// <summary>
        /// Элемент генерирующий поток
        /// </summary>
        Extractor,
        /// <summary>
        /// Элеиегь потребляющий поток (хранилище)
        /// </summary>
        Consumer,
        /// <summary>
        /// Установка
        /// </summary>
        Installation
    }

    public class NodeInfo
    {
        public string Id { get; private set; }
        public NodeType Type { get; set; }
        /// <summary>
        /// Используется только для установок
        /// </summary>
        public FlowElementScope Scope { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="scope">Используется только для установок (Installation)</param>
        public NodeInfo(string id, NodeType type, FlowElementScope scope)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentOutOfRangeException(nameof(id));

            Id = id;
            Type = type;
            Scope = scope;
        }
    }
}
