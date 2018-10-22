using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace QuantEngine
{
    internal class DataManager
    {
        //策略持仓
        private static readonly string SQL_CREATE_T_POSITION =
            @"CREATE TABLE [t_position](
              [id] int PRIMARY KEY, 
              [strategy_name] varchar(255), 
              [instrument_id] varchar(20), 
              [position] int, 
              [last_time] datetime);";

        //订单
        private static readonly string SQL_CREATE_T_ORDER =
            @"CREATE TABLE [t_order](
              [id] int PRIMARY KEY, 
              [strategy_name] varchar(255), 
              [instrument_id] varchar(20), 
              [direction] varchar(20), 
              [price] float, 
              [volume] int, 
              [volume_traded], 
              [order_time] datetime);";

        private static DataManager instance = new DataManager();
        public static DataManager Instance
        {
            get
            {
                return instance;
            }
        }
        private DataManager()
        {
            CreateDb();
            mQueue.OnMessage += _onMessage;
        }

        private void CreateDb()
        {
            string path = System.Environment.CurrentDirectory + @"/Data/QuantData.db";
            if (File.Exists(path))
            {
                return;
            }

            SQLiteHelper.CreateDBFile("QuantData.db");
            SQLiteHelper.ExecuteNonQuery(SQL_CREATE_T_POSITION);
            SQLiteHelper.ExecuteNonQuery(SQL_CREATE_T_ORDER);
        }

        private MessageQueue mQueue = new MessageQueue(10000);
        
        private void _onMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.position:
                    Position p = (Position)message.Value;
                    savePosition(p);
                    break;
                case MessageType.order:
                    Order o = (Order)message.Value;
                    saveOrder(o);
                    break;
            }
        }

        //持仓
        public int GetPosition(string strategyName)
        {
            string sql = @" select position from t_position where strategy_name = '"+strategyName+"'";
            return (int)SQLiteHelper.ExecuteScalar(sql);
        }
        public void SetPosition(Position position)
        {
            mQueue.add(MessageType.position, position);
        }
        private void savePosition(Position p)
        {
            dsfadsfdsaf
        }

        //订单
        public void AddOrder(Order order)
        {
            mQueue.add(MessageType.order, order);
        }
        private void saveOrder(Order o)
        {
            asdfadsf
        }
    }
}
