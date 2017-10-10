using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace spy
{
    public class TextLog
    {
        public string Path { get; set; }
        StreamWriter sw;
        public TextLog(string path)
        {
            Path = path;
            
        }
        public void Write(string sentence)
        {
            sw = new StreamWriter(Path, true);
            sw.WriteLine(DateTime.Now.ToLongDateString()+":"+sentence);
            sw.Close();
        }
    }
}
