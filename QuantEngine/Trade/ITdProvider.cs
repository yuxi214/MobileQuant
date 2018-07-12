using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public interface ITdProvider
    {
        //登陆
        bool Login();
        //登出
        bool Logout();
        //是否登陆
        bool IsLogin();
        //发送订单
        bool SendOrder(SubOrder subOrder);
        //撤销订单
        bool CancelOrder(SubOrder subOrder);
        //成交回报
        event RtnTrade OnTrade;
        //订单回报
        event RtnOrder OnOrder;
        //发单失败
        event RtnOrderFaild OnOrderFaild;
        //撤单失败
        event RtnCancelFaild OnCancelFaild;
    }

    //委托
    public delegate void RtnOrder(Order order);
    public delegate void RtnOrderFaild(Order order);
    public delegate void RtnCancelFaild(Order order);
    public delegate void RtnTrade(Trade trade);
}
