using System;
using System.IO;

namespace SepcReptile
{
    public class TextLog
    {
        public string Path { get; set; }
        private StreamWriter _sw;
        public TextLog(string path)
        {
            Path = path;
            
        }
        public void Write(string sentence)
        {
            _sw = new StreamWriter(Path, true);
            _sw.WriteLine(DateTime.Now.ToLongDateString()+":"+sentence);
            _sw.Close();
        }
    }
}
