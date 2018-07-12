using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    internal class Position
    {
        //合约名
        private string instrumentID = string.Empty;
        //今多
        private int tdLongQty = 0;
        //今多冻结
        private int tdLongFrozen = 0;
        //多
        private int longQty = 0;
        //多冻结
        private int longFrozen = 0;
        //今空
        private int tdShortQty = 0;
        //今空冻结
        private int tdShortFrozen = 0;
        //空
        private int shortQty = 0;
        //空冻结
        private int shortFrozen = 0;

        public int TdLongQty
        {
            get
            {
                return tdLongQty;
            }

            set
            {
                tdLongQty = value > 0 ? value : 0;
            }
        }

        public int TdLongFrozen
        {
            get
            {
                return tdLongFrozen;
            }

            set
            {
                tdLongFrozen = value > 0 ? value : 0;
            }
        }

        public int LongQty
        {
            get
            {
                return longQty;
            }

            set
            {
                longQty = value > 0 ? value : 0;
            }
        }

        public int LongFrozen
        {
            get
            {
                return longFrozen;
            }

            set
            {
                longFrozen = value > 0 ? value : 0;
            }
        }

        public int TdShortQty
        {
            get
            {
                return tdShortQty;
            }

            set
            {
                tdShortQty = value > 0 ? value : 0;
            }
        }

        public int TdShortFrozen
        {
            get
            {
                return tdShortFrozen;
            }

            set
            {
                tdShortFrozen = value > 0 ? value : 0;
            }
        }

        public int ShortQty
        {
            get
            {
                return shortQty;
            }

            set
            {
                shortQty = value > 0 ? value : 0;
            }
        }

        public int ShortFrozen
        {
            get
            {
                return shortFrozen;
            }

            set
            {
                shortFrozen = value > 0 ? value : 0;
            }
        }

        public Position(string instrumentID)
        {
            this.instrumentID = instrumentID;
        }


    }
}
