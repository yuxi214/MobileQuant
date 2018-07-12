using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public interface IMdProvider
    {
        //登陆
        bool Login(Account account);
        //登出
        void Logout();
        //是否登陆
        bool IsLogin();
        //订阅行情
        void SubscribeMarketData(String InstrumentID);
        //退订行情
        void UnSubscribeMarketData(String InstrumentID);
    }

    public class BaseMdProvider
    {
        //行情回报
        public event RtnTick OnTick;
    }

    //委托
    public delegate void RtnTick(Tick tick);
}
