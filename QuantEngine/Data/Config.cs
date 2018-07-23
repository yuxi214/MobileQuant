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
        private Account myAccount;

        public Account MyAccount
        {
            get
            {
                return myAccount;
            }

            set
            {
                myAccount = value;
            }
        }
    }
}
