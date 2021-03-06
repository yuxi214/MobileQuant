﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace DataReceiver {
    internal class ConfigUtils {
        internal static Config Config {
            get {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";
                Config config;
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    config = JsonConvert.DeserializeObject<Config>(json);
                } else {
                    config = new Config();
                    config.MyMdAccount = new Account("tcp://218.202.237.33:10001", "9999", "123456", "123456");
                    config.MyTdAccount = new Account("tcp://218.202.237.33:10001", "9999", "123456", "123456");
                    config.MysqlConnectString = "Database='QuantDB';Data Source='localhost';User Id='root';Password='root';charset='utf8';pooling=true";
                    File.Create(path).Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
                }
                return config;
            }
        }
    }
    internal class Config
    {
        private Account myTdAccount;
        private Account myMdAccount;
        private string mysqlConnectString;

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

        public string MysqlConnectString {
            get {
                return mysqlConnectString;
            }

            set {
                mysqlConnectString = value;
            }
        }
    }
}
