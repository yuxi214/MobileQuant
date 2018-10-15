using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace QuantEngine
{
    public class Utils
    {

        //日志
        public static void Log(string content)
        {
            WriteLogs("Logs", "", content);
        }
        public static void WriteLogs(string fileName, string type, string content)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + fileName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //path = path + "\\" + DateTime.Now.ToString("yyyyMMdd");
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path))
                {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + type + "-->" + content;
                    Console.WriteLine(c);
                    sw.WriteLine(c);
                    //  sw.WriteLine("----------------------------------------");
                    sw.Close();
                }
            }
        }

        //配置
        internal static Config Config
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";
                Config config;
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    config = JsonConvert.DeserializeObject<Config>(json);
                }
                else
                {
                    config = new Config();
                    config.MyTdAccount = new Account("tcp://218.202.237.33:10000", "9999", "123456", "123456");
                    config.MyMdAccount = new Account("tcp://218.202.237.33:10001", "9999", "123456", "123456");
                    File.Create(path).Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
                }
                return config;
            }
        }
    }
}
