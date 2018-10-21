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

        //执行线程
        Thread mThread;
        private void run()
        {
            if(mThread == null || !mThread.IsAlive)
            {
                mThread = new Thread(()=> {
                    while (true)
                    {
                        try
                        {
                            execute();
                        }
                        catch (Exception ex)
                        {
                            Utils.Log(ex.StackTrace);
                        }
                    }
                });
            }
        }
        private void execute()
        {
            kjlkj;l
        }

        //持仓
        public int GetPosition(string strategyName)
        {
            string sql = @" select position from t_position where strategy_name = '"+strategyName+"'";
            return (int)SQLiteHelper.ExecuteScalar(sql);
        }
        private BlockingCollection<Position> mPositionQueue = new BlockingCollection<Position>();
        public void SetPosition(Position position)
        {
            //最多缓存1000条
            if(mPositionQueue.Count < 1000)
            {
                mPositionQueue.TryAdd(position, 1000);
            }
        }

        //订单
        private BlockingCollection<Order> mOrderQueue = new BlockingCollection<Order>();
        public void AddOrder(Order order)
        {
            if(mPositionQueue.Count < 1000)
            {
                mOrderQueue.TryAdd(order, 1000);
            }
        }
    }
}
