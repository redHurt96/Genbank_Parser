using System.Collections.Generic;

namespace GenbankParser
{
    public static class DictExtensions
    {
        public static string GetOfDefault(this Dictionary<string, string> dict, string key) =>
            dict.ContainsKey(key)
                ? dict[key]
                : "";
    }
}
