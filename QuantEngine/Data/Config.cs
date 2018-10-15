using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace QuantEngine
{
    internal class Config
    {
        private Account myTdAccount;
        private Account myMdAccount;

        public Account MyTdAccount
        {
            get
            {
                return myTdAccount;
            }

            set
            {
                myTdAccount = value;
            }
        }

        public Account MyMdAccount
        {
            get
            {
                return myMdAccount;
            }

            set
            {
                myMdAccount = value;
            }
        }
    }
}
