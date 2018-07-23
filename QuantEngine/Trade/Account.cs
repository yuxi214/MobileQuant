﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace QuantEngine
{
    public class Account
    {
        private string server;
        private string broker;
        private string investor;
        private string password;

        public Account(string server, string broker, string investor, string password)
        {
            this.server = server;
            this.broker = broker;
            this.investor = investor;
            this.password = password;
        }

        public string Server
        {
            get
            {
                return server;
            }
        }

        public string Broker
        {
            get
            {
                return broker;
            }
        }

        public string Investor
        {
            get
            {
                return investor;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
        }
    }
}
