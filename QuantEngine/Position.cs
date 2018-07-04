using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    class Position
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

        public Position(string instrumentID)
        {
            this.instrumentID = instrumentID;
        }

        public string InstrumentID
        {
            get
            {
                return instrumentID;
            }
        }

        public int TdLongQty
        {
            get
            {
                return tdLongQty;
            }
        }

        public int TdLongFrozen
        {
            get
            {
                return tdLongFrozen;
            }
        }

        public int LongQty
        {
            get
            {
                return longQty;
            }
        }

        public int LongFrozen
        {
            get
            {
                return longFrozen;
            }
        }

        public int TdShortQty
        {
            get
            {
                return tdShortQty;
            }
        }

        public int TdShortFrozen
        {
            get
            {
                return tdShortFrozen;
            }
        }

        public int ShortQty
        {
            get
            {
                return shortQty;
            }
        }

        public int ShortFrozen
        {
            get
            {
                return shortFrozen;
            }
        }

        //多开
        public void LongOpen(int vol)
        {
            tdLongQty += vol;
        }

        //空开
        public void ShortOpen(int vol)
        {
            tdShortQty += vol;
        }

        //今多开
        public void TdLongOpen(int vol)
        {
            tdLongQty += vol;
        }

        //今空开
        public void TdShortOpen(int vol)
        {
            tdShortQty += vol;
        }

        //多平
        public void LongClose(int vol)
        {
            longQty -= vol;
            if(longQty < 0)
            {
                longQty = 0;
            }

        }

        //空平
        public void ShortClose(int vol)
        {
            shortQty -= vol;
            if(shortQty < 0)
            {
                shortQty = 0;
            }
        }

        //今多平
        public void TdLongClose(int vol)
        {
            tdLongQty -= vol;
            if(tdLongQty < 0)
            {
                tdLongQty = 0;
            }
        }

        //今空平
        public void TdShortClose(int vol)
        {
            tdShortQty -= vol;
            if(tdShortQty < 0)
            {
                tdShortQty = 0;
            }
        }
    }
}
