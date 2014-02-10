using System.Collections.Generic;
using System.IO;

namespace DC
{
   static class StreamWorker
    {
       /// <summary>
       /// Отримання з файлу даних, що складаються з одного запису
       /// </summary>
       /// <param name="file">Шлях до файлу</param>
       /// <returns></returns>
       static public List<string> ReadList(string file)
        {
            if (!File.Exists(file))
            {
                StreamWriter creator = new StreamWriter(file);
                creator.Close();
            }

            List<string> result = new List<string>();
            StreamReader reader = new StreamReader(file);
            while (true)
            {
                string read = reader.ReadLine();
                if (read == null)
                { break; }
                
                result.Add(read);
            }
            reader.Close();
            return result;
        }

       /// <summary>
       /// Отримання даних, які складаються з двох записів, розділених знаком '|'
       /// </summary>
       /// <param name="file">Шлях до файлу</param>
       /// <returns></returns>
       static public Dictionary<string,string> ReadDictionary(string file)
       {
           if (!File.Exists(file))
           {
               StreamWriter creator = new StreamWriter(file);
               creator.Close();
           }
           Dictionary<string, string> result = new Dictionary<string, string>();
           StreamReader reader = new StreamReader(file);
           while (true)
           {
               string read = reader.ReadLine();
               if (read == null)
               { break; }

               result.Add(read.Split('|')[0], read.Split('|')[1]);
           }
           reader.Close();
           return result;
       }
    }
}
