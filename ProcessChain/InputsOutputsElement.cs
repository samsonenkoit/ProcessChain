using ProcessChain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleToAttribute("ProcessChainTest")]
namespace ProcessChain
{
    /// <summary>
    /// Основной тип усзлов схемы, может содержать как входные, так и выходные соединения
    /// </summary>
    public class InputsOutputsElement : NodeElement
    {
        private readonly NodeScope _scope;
        private readonly Func<IEnumerable<NodeConnection>, IEnumerable<NodeConnection>, IDictionary<string, double>> _distributionStrategy;

        private Dictionary<string, NodeConnection> _inputConnections;
        private Dictionary<string, NodeConnection> _outputConnections;

        /// <summary>
        /// Ограничение узла
        /// </summary>
        public NodeScope Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// Величина потока узла
        /// </summary>
        public override double FlowRate
        {
            get
            {
                return _inputConnections.Sum(t => t.Value.CurrentRate);
            }
        }

        internal InputsOutputsElement(string id, NodeScope scope,
            Func<IEnumerable<NodeConnection>, IEnumerable<NodeConnection>, IDictionary<string, double>> distributionStrategy) : base(id)
        {
            if (distributionStrategy == null) throw new ArgumentNullException(nameof(distributionStrategy));
            if (scope == null) throw new ArgumentNullException(nameof(scope));

            _scope = scope;
            _distributionStrategy = distributionStrategy;
        }

        /// <summary>
        /// Устанавливает соединения узла
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        internal void SetConnections(IEnumerable<NodeConnection> inputs, IEnumerable<NodeConnection> outputs)
        {
            if (_inputConnections != null) throw new InvalidOperationException();
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (outputs == null) throw new ArgumentNullException(nameof(outputs));

            _inputConnections = new Dictionary<string, NodeConnection>();
            foreach (var input in inputs)
                _inputConnections.Add(input.Id, input);

            _outputConnections = new Dictionary<string, NodeConnection>();
            foreach (var output in outputs)
                _outputConnections.Add(output.Id, output);

        }

        /// <summary>
        /// Обновляет параметры исходящих соединений
        /// </summary>
        /// <param name="quotas"></param>
        /// <returns></returns>
        public SchemeRateUpdateResult UpdateOutputConnectionsQuota(IDictionary<string, NodeConnectionQuota> quotas)
        {
            if (quotas == null) throw new ArgumentNullException();

            foreach(var quota in quotas)
            {
                if (!_outputConnections.ContainsKey(quota.Key)) continue;

                _outputConnections[quota.Key].Quota = quota.Value;
            }

            return UpdateFlowRates();
        }

        internal override SchemeRateUpdateResult UpdateFlowRates()
        {            
            double inputFlowRate = _inputConnections.Values.Sum(t => t.CurrentRate);

            //проверяем что входной поток в рамках допустимого
            if (inputFlowRate > _scope.RateValueMax)
                return new SchemeRateUpdateResult(new SchemeRestriction(FlowRestrictionTypes.FlowRateValueMaximum,
                    this, inputFlowRate - _scope.RateValueMax));

            double minOutputFlowRate = _outputConnections.Values.Where(t => t.Quota.IsDirectValue).Sum(t => t.Quota.DirectValue);

            //проверяем что входной поток >= чем выходной
            if (inputFlowRate < minOutputFlowRate)
                return new SchemeRateUpdateResult(new SchemeRestriction(FlowRestrictionTypes.InputFlowNotEqualOutput,
                    this, minOutputFlowRate - inputFlowRate));

            //считаем новые выходные потоки
            var newFlowRates = _distributionStrategy(_inputConnections.Select(t => t.Value).ToList(),
                _outputConnections.Select(t => t.Value).ToList());

            foreach(var output in _outputConnections)
            {
                var result = output.Value.FlowRateUpdate(newFlowRates[output.Key]);

                if (!result.IsSuccess)
                    return result;
            }

            return new SchemeRateUpdateResult();
        }
        
    }
}
