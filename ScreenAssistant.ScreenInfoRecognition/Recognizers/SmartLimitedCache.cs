using System.Collections.Generic;
using System.Linq;
using TiqUtils.Serialize;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers
{
    internal sealed class SmartLimitedCache<TK, TV>
    {

        public SmartLimitedCache(int limit) : this()
        {
            this.Limit = limit;
        }

        public SmartLimitedCache()
        {
            this.Cache = new Dictionary<TK, SmartCacheNode<TV>>();
        }

        public int Limit { get; set; } = 50;

        public Dictionary<TK, SmartCacheNode<TV>> Cache { get; set; }

        public void SaveToFile(string fileName)
        {
            this.SerializeDataJson(fileName);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            if (this.Cache.TryGetValue(key, out var smartValue))
            {
                value = smartValue.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void ScoreHit(TK key)
        {
            this.Cache[key].Hits++;
        }

        public void Add(TK key, TV value)
        {
            if (this.Cache.Count >= this.Limit)
            {
                var leastKey = this.Cache.OrderBy(x => x.Value.Hits).First().Key;
                this.Cache.Remove(leastKey);
            }

            if (this.Cache.ContainsKey(key))
            {
                this.Cache[key].Value = value;
                this.Cache[key].Hits = 1;
            }
            else
            {
                this.Cache[key] = new SmartCacheNode<TV>(value);
            }
        }

        public static SmartLimitedCache<TK, TV> RestoreFromFile(string fileName, int limit)
        {
            return Json.DeserializeDataFromFile<SmartLimitedCache<TK, TV>>(fileName) ?? new SmartLimitedCache<TK, TV>(limit);
        }

        public class SmartCacheNode<T>
        {
            public SmartCacheNode(T value)
            {
                this.Value = value;
            }

            public T Value { get; set; }

            public int Hits { get; set; } = 1;

            public override string ToString()
            {
                return $"V: {this.Value}, H: {this.Hits}";
            }
        }

    }

}
