using ProcessChain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /*
        В качестве узлов в схеме могут выступать 3 типа объектов:
        
        - Extractor - является начальным узлом схемы, генерирует определенный (настраиваемый) объем вещества,
        может иметь только исходящие соединения.
        - Consumer - хранилище, может иметь только входящие соединения, является конечным узлом схема.
        - Installations - основной тип для узлов схемы, может иметь не ограниченное количество входящих и исходящих соединений,
        для узлов этого типа выполняется проверка того что объем входного потока == выходному. Для каждого выходного потока можно
        задвать процент (процент входного потока, который подается в соединение) и DirectValue (фиксированный объем потока, который 
        долэен подаваться на соединение, имеет приоритет перед процентом)

    */

    /// <summary>
    /// Представляет связанную систему узлов. Позволяет задвать параметры системы и считает допустимость параметров. 
    /// </summary>
    public class Scheme
    {
        /// <summary>
        /// Список источников
        /// </summary>
        public Dictionary<string, Extractor> Extractors { get; private set; }
        /// <summary>
        /// Список установок
        /// </summary>
        public Dictionary<string, InputsOutputsElement> Installations { get; private set; }
        /// <summary>
        /// Хранилища
        /// </summary>
        public Dictionary<string, Consumer> Consumers { get; private set; }

        /// <summary>
        /// Соединения
        /// </summary>
        internal Dictionary<string, NodeConnection> Connections { get; set; }

        internal Scheme(Dictionary<string, Extractor> extractors, Dictionary<string, InputsOutputsElement> installations, 
            Dictionary<string, Consumer> consumers, Dictionary<string, NodeConnection> connections)
        {
            if (extractors == null) throw new ArgumentNullException(nameof(extractors));
            if (installations == null) throw new ArgumentNullException(nameof(installations));
            if (consumers == null) throw new ArgumentNullException(nameof(consumers));
            if (connections == null) throw new ArgumentNullException(nameof(connections));

            Extractors = extractors;
            Installations = installations;
            Consumers = consumers;
            Connections = connections;
        }

        /// <summary>
        /// Возвращает все узлы.
        /// </summary>
        /// <returns></returns>
        public IList<NodeElement> GetAllNodes()
        {
            List<NodeElement> nodes = new List<NodeElement>();
            nodes.AddRange(Extractors.Select(t => t.Value));
            nodes.AddRange(Installations.Select(t => t.Value));
            nodes.AddRange(Consumers.Select(t => t.Value));

            return nodes;
        }
    }
}
