using System;
using System.IO;

namespace SepcReptile
{
    public class TextLog
    {
        private StreamWriter _sw;
        public TextLog(string path)
        {
            _sw = new StreamWriter(path, true);
        }
        public void Write(string sentence)
        {
            
            _sw.WriteLine(DateTime.Now.ToLongDateString()+":"+sentence);
            _sw.Close();
        }
    }
}
