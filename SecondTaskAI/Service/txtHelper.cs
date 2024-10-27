using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SecondTaskAI
{
    internal static class txtHelper
    {
        internal static async Task<string> ReadFileAsync(string path)
        {
            string result;
            using (StreamReader reader = new StreamReader(path))
            {
                result = await reader.ReadToEndAsync();
            }
            return result;
        }
        internal static async Task<List<string>> ReadFileLinesAsync(string path)
        {
            List<string> result = new List<string>();
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                return null;
            }
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                    result.Add(line);
            }
            return result;
        }

        internal static async void WriteFileAsync(string path, string text)
        {
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = Encoding.Default.GetBytes(text);
                await fstream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        internal static async void JoinFile(List<string> paths, string filename = "allvkid")
        {
            foreach (string path in paths)
            {
                
                if (File.Exists(path))
                {
                    List<string> t = await ReadFileLinesAsync(path);
                    WriteFileLines($@"data\dataUser\{filename}.txt", t, true);
                }
                
            }
        }
        internal static void WriteFileLines(string path, List<string> data, bool append = false)
        {
            if (!append) File.WriteAllLines(path, data);
            else File.AppendAllLines(path, data);
        }
        internal static bool FileExists(string path) => File.Exists(path);
    }
}
