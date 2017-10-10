using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

namespace spy
{
    class RegexpOperation
    {
        public static string[] MultiRegex(string searcher, string reg)
        {
            Regex r = new Regex(reg);
            MatchCollection mc = r.Matches(searcher);
            string[] vals = new string[mc.Count];
            for (int i = 0; i < vals.Length; ++i)
            {
                vals[i] = mc[i].Value;
            }
            return vals;
        }
        public static string SingleRegex(string searcher, string reg)
        {
            Regex r = new Regex(reg);
            Match m = r.Match(searcher);
            return m.Value;
        }
        public static string GroupSingleRegex(string searcher, string reg)
        {
            Regex r = new Regex(reg);
            Match m = r.Match(searcher);
            return m.Groups[1].Value;
        }
        public static string[] GroupMultiRegex(string searcher, string reg)
        {
            Regex r = new Regex(reg);
            MatchCollection mc = r.Matches(searcher);
            string[] vals = new string[mc.Count];
            for (int i = 0; i < vals.Length; ++i)
            {
                var x = mc[i].Groups;
                vals[i] = x[1].Value;
            }
            return vals;
        }
        public static DataTable Create(string source, string pattern, bool ingroup = false, int positions = 0)
        {
            DataTable dt = new DataTable();
            Regex rg = new Regex(pattern);
            MatchCollection mc = rg.Matches(source);
            if(ingroup)
            {
                dt.Columns.AddRange(new DataColumn[positions]);
                for(int i=0;i<mc.Count;++i)
                {
                    DataRow dr = dt.NewRow();
                    for(int j=0;j<positions;++j)
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
            Regex r = new Regex(reg);
            return r.IsMatch(searcher);
        }
        public static string UnicodeDeserialize(string str)
        {
            string[] bytes = str.Split(new string[] { "\\u" }, StringSplitOptions.RemoveEmptyEntries);
            string result = new string(bytes.Select(x => (char)Convert.ToInt16(x, 16)).ToArray());
            return result;
        }
    }
}
