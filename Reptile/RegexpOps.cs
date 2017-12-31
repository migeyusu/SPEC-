using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reptile
{
    internal class RegexpOperation
    {
        public static string[] MultiRegex(string searcher, string reg)
        {
            var r = new Regex(reg);
            var mc = r.Matches(searcher);
            var vals = new string[mc.Count];
            for (var i = 0; i < vals.Length; ++i)
            {
                vals[i] = mc[i].Value;
            }
            return vals;
        }
        public static string SingleRegex(string searcher, string reg)
        {
            var r = new Regex(reg);
            var m = r.Match(searcher);
            return m.Value;
        }
        public static string GroupSingleRegex(string searcher, string reg)
        {
            var r = new Regex(reg);
            var m = r.Match(searcher);
            return m.Groups[1].Value;
        }
        public static string[] GroupMultiRegex(string searcher, string reg)
        {
            var r = new Regex(reg);
            var mc = r.Matches(searcher);
            var vals = new string[mc.Count];
            for (var i = 0; i < vals.Length; ++i)
            {
                var x = mc[i].Groups;
                vals[i] = x[1].Value;
            }
            return vals;
        }
        public static DataTable Create(string source, string pattern, bool ingroup = false, int positions = 0)
        {
            var dt = new DataTable();
            var rg = new Regex(pattern);
            var mc = rg.Matches(source);
            if(ingroup)
            {
                dt.Columns.AddRange(new DataColumn[positions]);
                for(var i=0;i<mc.Count;++i)
                {
                    var dr = dt.NewRow();
                    for(var j=0;j<positions;++j)
                    {
                        dr[j] = mc[i].Groups[j + 1].Value;
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public static bool RegexTest(string searcher,string reg)
        {
            var r = new Regex(reg);
            return r.IsMatch(searcher);
        }
        public static string UnicodeDeserialize(string str)
        {
            var bytes = str.Split(new[] { "\\u" }, StringSplitOptions.RemoveEmptyEntries);
            var result = new string(bytes.Select(x => (char)Convert.ToInt16(x, 16)).ToArray());
            return result;
        }
    }
}
