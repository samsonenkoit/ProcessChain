using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Interface
{
    /// <summary>
    /// Базовый для элемента схемы
    /// </summary>
    public abstract class Element
    {

        public string Id { get; private set; }

        public Element(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new IndexOutOfRangeException(nameof(id));

            Id = id;
        }
    }
}
