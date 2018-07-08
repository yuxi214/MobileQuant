using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public interface IMdProvider
    {
        //连接
        bool Connect();
        //登陆
        bool UserLogin();
        //登出
        bool UserLogout();
        //是否登陆
        bool Login();
        //订阅行情
        bool SubscribeMarketData(String InstrumentID);
        //退订行情
        bool UnSubscribeMarketData(String InstrumentID);
    }
}
