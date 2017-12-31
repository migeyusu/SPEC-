using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Reptile
{
    internal class UrlCreate
    {
        public static string ParameterEncode(string str)
        {
            var seed = new StringBuilder();
            var results = Encoding.UTF8.GetBytes(str);
            for (var i = 0; i < results.Length; ++i)
                seed.Append(@"%" + Convert.ToString(results[i], 16));
            return seed.ToString();
        }
        public void HashtableSerialize(Hashtable ht,string filepath)
        {
            var fs = new FileStream(filepath, FileMode.Create);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, ht);
            fs.Close();
        }
        public Hashtable HashtableDeserialize(string filepath)
        {
            var fs = new FileStream(filepath, FileMode.OpenOrCreate);
            var bf = new BinaryFormatter();
            var ht = (Hashtable)bf.Deserialize(fs);
            fs.Close();
            return ht;
        }
    }
}