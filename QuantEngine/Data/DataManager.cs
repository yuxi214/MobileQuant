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
            mQueue.OnMessage += _onMessage;
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
        public int GetPosition(string strategyName,string instrumentID)
        {
            string sql = $@" select position from t_position where strategy_name = '{strategyName}' and instrument_id = '{instrumentID}'";
            object vol = SQLiteHelper.ExecuteScalar(sql);
            return vol == null ? 0 : (int)(long)vol;
        }
        public void SetPosition(Position position)
        {
            mQueue.add(MessageType.position, position);
        }
        private void savePosition(Position p)
        {
            string sql = $@"replace into [t_position]
                            ([strategy_name], [instrument_id], [position], [last_time]) 
                            values
                            ('{p.StrategyName}','{p.InstrumentID}',{p.Vol},'{p.LastTime}')";
            SQLiteHelper.ExecuteNonQuery(sql);
        }

        //订单
        public void AddOrder(Order order)
        {
            mQueue.add(MessageType.order, order);
        }
        private void saveOrder(Order o)
        {
            string direction = o.Direction == DirectionType.Buy ? "多" : "空";
            string sql = $@"insert into [t_order]
                            ([strategy_name], [instrument_id], [direction], [price], [volume], [volume_traded], [order_time])
                            values
                            ('{o.Strategy.GetType().Name}','{o.InstrumentID}','{direction}',{o.Price},{o.Volume},{o.Volume-o.VolumeLeft},'{o.OrderTime}')";
            SQLiteHelper.ExecuteNonQuery(sql);
        }
    }
}
