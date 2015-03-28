using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NekoHacks
{
    public static class Util
    {
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false)
                dict.Add(key, value);
            return dict;
        }
    }
}
