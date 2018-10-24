using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Newtonsoft.Json;

namespace QuantEngine
{
    public class Utils
    {
        //日志
        private class Log
        {
            string fileName;
            string content;

            public Log(string fileName, string content)
            {
                this.fileName = fileName;
                this.content = content;
            }

            public string FileName
            {
                get
                {
                    return fileName;
                }
            }

            public string Content
            {
                get
                {
                    return content;
                }
            }
        }
        private static Thread mThread;
        private static BlockingCollection<Log> mQueue = new BlockingCollection<Log>();
        internal static void EnginLog(string content)
        {
            Log l = new Log("enginLog", content);
            log(l);
        }
        internal static void UserLog(string content)
        {
            Log l = new Log("userLog", content);
            log(l);
        }
        private static void log(Log log)
        {
            //最多缓存1000条log
            if (mQueue.Count < 1000)
            {
                mQueue.TryAdd(log);
            }

            //异步写日志
            if (mThread == null || !mThread.IsAlive)
            {
                mThread = new Thread(() =>
                {
                    while (true)
                    {
                        Log l = mQueue.Take();
                        try
                        {
                            writeLogs(l.FileName, "", l.Content);
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }

                });
                mThread.Start();
            }
        }
        private static void writeLogs(string fileName, string type, string content)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory +"\\log\\"+ fileName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
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
