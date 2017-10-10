using System;
using System.IO;

namespace SepcReptile
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
