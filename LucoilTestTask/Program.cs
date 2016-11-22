using ProcessChain;
using ProcessChain.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucoilTestTask
{
    /*
     * Демонстрационный пример работы с библиотекой ProcessChain
     */
    class Program
    {
        static void Main(string[] args)
        {
            Demo();
            Console.ReadLine();
        }

        /// <summary>
        /// Строит схему с тестовыми данными
        /// </summary>
        /// <returns></returns>
        private static Scheme BuldTestData()
        {
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

            var scheme = SchemeBuilder.Build(nodes, conns);

            return scheme;
        }

        /// <summary>
        /// Выводит данные схемы на консоль
        /// </summary>
        /// <param name="scheme"></param>
        private static void Print(Scheme scheme)
        {
            foreach(var node in scheme.GetAllNodes())
            {
                Console.WriteLine($"{node.Id} flowrate: {node.FlowRate}");
            }
        }

        /// <summary>
        /// Пример работы с схемой
        /// </summary>
        private static void Demo()
        {
            var scheme = BuldTestData();

            #region Step 1
            Console.WriteLine("Update flow rate: node n10, new flow rate 100");

            //Задаем величину потока = 100, на источнике с Id = n10
            var result = scheme.Extractors["n10"].FlowRateUpdate(100);

            Console.WriteLine(result.ToString());
            Print(scheme);

            Console.WriteLine();

            #endregion

            #region Step 2
            Console.WriteLine("Update flow rate: node n11, new flow rate 200");

            //Задаем величину потока = 200, на источнике с Id = n11
            result = scheme.Extractors["n11"].FlowRateUpdate(200);

            Console.WriteLine(result.ToString());
            Print(scheme);

            Console.WriteLine();
            #endregion

            #region Step 3
            Console.WriteLine("Update connections quota: node n5");

            //Создаем новый список параметров соединений
            var newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n5n7", new NodeConnectionQuota(40));
            newQuota.Add("n5n9", new NodeConnectionQuota(40));
            newQuota.Add("n5n8", new NodeConnectionQuota(20));

            Console.WriteLine("New quotas");
            foreach (var quota in newQuota)
                Console.WriteLine($"Connection: {quota.Key}, value: {quota.Value.ToString()}");

            //обновляем параметры соединений у установки с Id = n5
            result = scheme.Installations["n5"].UpdateOutputConnectionsQuota(newQuota);

            Console.WriteLine(result.ToString());
            Print(scheme);

            Console.WriteLine();

            #endregion

            #region Step 4
            Console.WriteLine("Update connections quota: node n7");
            newQuota = new Dictionary<string, NodeConnectionQuota>();
            newQuota.Add("n7n9", new NodeConnectionQuota(100, 139));

            Console.WriteLine("New quotas");
            foreach (var quota in newQuota)
                Console.WriteLine($"Connection: {quota.Key}, value: {quota.Value.ToString()}");

            //Обновляем параметры соединений у установки с Id = n7
            result = scheme.Installations["n7"].UpdateOutputConnectionsQuota(newQuota);

            //уведомляем об ошибке
            if(!result.IsSuccess)
                Console.WriteLine("Error!!");

            Console.WriteLine(result.ToString());
            Print(scheme);

            Console.WriteLine();

            #endregion
        }
    }
}
