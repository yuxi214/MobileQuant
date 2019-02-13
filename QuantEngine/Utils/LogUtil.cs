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
    public class LogUtils
    {

        private static Thread mThread;
        private static MessageQueue mMessageQueue;
        internal static void EnginLog(string content)
        {
            Log l = new Log(LogType.Enginlog, content);
            log(l);
        }
        internal static void UserLog(string content)
        {
            Log l = new Log(LogType.UserLog, content);
            log(l);
        }
        private static void log(Log log)
        {
            if(mMessageQueue == null)
            {
                mMessageQueue = MessageQueue.Instance;
                mMessageQueue.OnMessage += (Message msg) => {
                    if(msg.Type == MessageType.log)
                    {
                        Log log2 = (Log)msg.Value;
                        writeLogs(log2.Type.ToString(), "", log2.Content);
                    }
                };
            }
            mMessageQueue.add(MessageType.log, log);
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
    }

    internal class Log
    {
        LogType type;
        string content;

        public Log(LogType type, string content)
        {
            this.type = type;
            this.content = content;
        }

        public LogType Type
        {
            get
            {
                return type;
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

    internal enum LogType
    {
        Enginlog,
        UserLog
    }
}
