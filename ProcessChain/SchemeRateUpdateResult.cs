using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain
{
    /// <summary>
    /// Результат обновления параметров схемы
    /// </summary>
    public class SchemeRateUpdateResult
    {
        /// <summary>
        /// Ошибка обновления
        /// </summary>
        public SchemeRestriction Restriction { get; private set; }

        public bool IsSuccess
        {
            get
            {
                return Restriction == null;
            }
        }

        public SchemeRateUpdateResult(SchemeRestriction restriction)
        {
            Restriction = restriction;
        }

        public SchemeRateUpdateResult(): this(null)
        {

        }

        public override string ToString()
        {
            return $"IsSuccess: {IsSuccess}, Restriction: {Restriction?.ToString()}";
        }
    }
}
