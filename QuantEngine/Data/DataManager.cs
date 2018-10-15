using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            SQLiteHelper.CreateDBFile("QuantData.db");
            SQLiteHelper.ExecuteNonQuery(SQL_CREATE_T_POSITION);
            SQLiteHelper.ExecuteNonQuery(SQL_CREATE_T_ORDER);
        }
    }
}
