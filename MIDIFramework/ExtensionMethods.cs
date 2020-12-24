using System.Collections.Generic;
using System.Linq;

namespace MIDIFramework
{
    public static class ExtensionMethods
    {
        public static List<T> Copy<T>(this IEnumerable<T> source)
        {
            var s = source.ToList();
            var t = new T[s.Count];
            s.CopyTo(t);
            return t.ToList();
        }
    }
}
