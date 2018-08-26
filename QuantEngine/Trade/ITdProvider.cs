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
        void Login(Account account);
        //登出
        void Logout();
        //是否登陆
        bool IsLogin();
        //发送订单
        void SendOrder(Order order);
        //撤销订单
        void CancelOrder(Order order);
    }
}
