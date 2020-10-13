using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace i3dm.tooling.Utils
{
    public class FileUtils
    {
        public static void SaveItems<T>(List<T> items, string filename)
        {
            var w = new StreamWriter(filename);

            foreach (var item in items)
            {
                var s = item.ToString();
                s = s.Trim('<').Trim('>');
                w.WriteLine(s);
            }
            w.Flush();
        }

        public static List<float> ReadFloats(string file)
        {
            var sr = new StreamReader(file);
            string data;
            var floats = new List<float>();
            while ((data = sr.ReadLine()) != null)
            {
                var read = float.Parse(data);
                floats.Add(read);
            }
            sr.Close();
            return floats;
        }

        public static List<Vector3> ReadVectors(string file)
        {
            var sr = new StreamReader(file);
            string data;
            var vectors = new List<Vector3>();
            while ((data = sr.ReadLine()) != null)
            {
                var read = data.Split(',');
                var x = float.Parse(read[0]);
                var y = float.Parse(read[1]);
                var z = float.Parse(read[2]);
                var v = new Vector3(x, y, z);
                vectors.Add(v);
            }
            sr.Close();
            return vectors;
        }
    }
}