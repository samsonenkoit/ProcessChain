using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChain.Extension
{
    public static class DictionaryExtension
    {
        public static IDictionary<TKey,Tvalue> AddRange<TKey,Tvalue>(this IDictionary<TKey,Tvalue> dictionary,
            IEnumerable<KeyValuePair<TKey,Tvalue>> coll)
        {
            foreach (var item in coll)
                dictionary.Add(item);

            return dictionary;
        }

    }
}
