using System.Collections.Generic;

namespace IaS.WorldBuilder.XML.mappers
{
    public class IdMapper
    {
        public static string Map(string from, string type, Dictionary<string, int> counts, bool present)
        {
            if (!present)
            {
                int count;
                if (!counts.TryGetValue(type, out count))
                {
                    counts.Add(type, -1);
                }
                count++;
                counts[type] = count;
                return string.Format("{0}_{1}", type, count);
            }
            return from;
        }

    }
}