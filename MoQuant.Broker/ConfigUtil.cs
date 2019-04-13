using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Newtonsoft.Json;

namespace MoQuant.Broker {
    public class ConfigUtils
    {
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

    internal class Config {
        private Account myTdAccount;
        private Account myMdAccount;

        public Account MyTdAccount {
            get {
                return myTdAccount;
            }

            set {
                myTdAccount = value;
            }
        }

        public Account MyMdAccount {
            get {
                return myMdAccount;
            }

            set {
                myMdAccount = value;
            }
        }
    }
}
