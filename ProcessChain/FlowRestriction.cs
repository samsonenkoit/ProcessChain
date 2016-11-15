using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Тип ограничения
    /// </summary>
    public enum FlowRestrictionTypes
    {        

        /// <summary>
        /// Значение потока больше допустимого максимума
        /// </summary>
        FlowRateValueMaximum = 1,

        /// <summary>
        /// Входной поток не равен выходному.
        /// </summary>
        InputFlowNotEqualOutput = 2
    }

    /// <summary>
    /// Ограничение обновления схемы
    /// </summary>
    public class FlowRestriction
    {
        /// <summary>
        /// Тип ограничения
        /// </summary>
        public FlowRestrictionTypes Type { get; private set; }
        /// <summary>
        /// Элемент, вызвавший ограничение
        /// </summary>
        public FlowElement Element { get; private set; }

        /// <summary>
        /// Величина показывает разницу между ожидаемым значением и фактическим, например если в узле величина
        /// входного потока 7, а выходного 5, то RestrictionValue будет равен 7 - 5 = 2
        /// </summary>
        public double RestrictionValue { get; private set; }

        public FlowRestriction(FlowRestrictionTypes type, FlowElement element, double restrictionValue)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            Type = type;
            Element = element;
            RestrictionValue = restrictionValue;
        }

        public override string ToString()
        {
            return $"Type: {Type}, NodeId: {Element.Id}, RestrictionValue: {RestrictionValue}";
        }
    }
}
