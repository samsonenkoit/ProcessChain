using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{

    public static class FlowRateDistributionStrategy
    {
        /// <summary>
        /// Ф-я распределения выходных потоков узла. Распределяет в максимальным соответствие с установленными процентами.
        /// (DirectValue имеет приоритет над процентом)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static IDictionary<string,double> MaxByQuotes(IEnumerable<FlowConnection> input, IEnumerable<FlowConnection> output)
        {
            if (input == null || !input.Any()) throw new ArgumentOutOfRangeException(nameof(input));
            if (output == null || !output.Any()) throw new ArgumentOutOfRangeException(nameof(output));

            var directedConn = output.Where(t => t.Quota.IsDirectValue).ToList();
            var notDirectedConn = output.Where(t => !t.Quota.IsDirectValue).ToList();

            double inputSumRate = input.Sum(t => t.CurrentRate);

            //минимальный выходной поток
            double minOutputRate = directedConn.Sum(t => t.Quota.DirectValue);

            if (inputSumRate < minOutputRate) throw new InvalidOperationException("Input flow rate is low");

            Dictionary<string, double> newRates = new Dictionary<string, double>();

            foreach(var con in directedConn)
            {
                newRates.Add(con.Id, con.Quota.DirectValue);
            }

            //поток, который еще не распределен
            double availableRate = inputSumRate - minOutputRate;
            // Коэффициент приведения процентов к 1
            double quotesSum = notDirectedConn.Sum(t => t.Quota.Percent);

            foreach(var con in notDirectedConn)
            {
                double rateOnOutput = availableRate * (con.Quota.Percent / quotesSum);
                newRates.Add(con.Id, rateOnOutput);
            }

            return newRates;
            
        }
    }
}
