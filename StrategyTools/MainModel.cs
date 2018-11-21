using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using QuantEngine;

namespace StrategyTools
{
    internal class MainModel
    {
        //单例
        private MainModel()
        {
            loadTables();
        }
        private static MainModel instance = new MainModel();
        internal static MainModel Instance
        {
            get
            {
                return instance;
            }
        }

        //
        private DataTable strategyTable;
        private Dictionary<string, DataTable> positionDic = new Dictionary<string, DataTable>();
        private Dictionary<string, DataTable> orderDic = new Dictionary<string, DataTable>();

        //
        private readonly string QUERY_POSITION = @"select [id],
                                                            [strategy_name], 
                                                            [instrument_id], 
                                                            [position], 
                                                            [last_time]
                                                            from [t_position]";

        private readonly string QUERY_ORDER = @"select [id], 
                                                        [strategy_name], 
                                                        [instrument_id], 
                                                        [direction], 
                                                        [price], 
                                                        [volume], 
                                                        [volume_traded], 
                                                        [order_time]
                                                        from [t_order]";

        internal DataTable StrategyTable
        {
            get
            {
                return strategyTable;
            }
        }

        //
        private void loadTables()
        {
            //加载当前策略
            strategyTable = new DataTable("strategys");
            strategyTable.Columns.Add("strategy_name", Type.GetType("System.String"));
            strategyTable.Columns.Add("useable", Type.GetType("System.Boolean"));

            //获取文件列表 
            string[] files = new string[] { };
            try
            {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\strategys");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

            //加载策略
            foreach (string f in files)
            {
                Assembly assembly = Assembly.LoadFrom(f);
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    //必须要是策略子类
                    if (!t.IsSubclassOf(typeof(BaseStrategy)))
                        continue;
                    //避免重复添加
                    if (strategyTable.Select($"strategy_name = '{t.Name}'").Length > 0)
                        continue;

                    strategyTable.Rows.Add(t.Name, true);
                }
            }

            //加载持仓
            DataTable positionTable = SQLiteHelper.GetDataTable(QUERY_POSITION);
            foreach(DataRow row in positionTable.Rows)
            {
                string strategyName = row.Field<string>("strategy_name");
                if (strategyTable.Select($"strategy_name = '{strategyName}'").Length == 0)
                    strategyTable.Rows.Add(strategyName, false);

                DataTable t;
                if (!positionDic.TryGetValue(strategyName,out t))
                {
                    t = positionTable.Clone();
                    positionDic.Add(strategyName, t);
                }
                t.Rows.Add(row.ItemArray);
            }

            //加载订单
            DataTable orderTable = SQLiteHelper.GetDataTable(QUERY_ORDER);
            foreach(DataRow row in orderTable.Rows)
            {
                string strategyName = row.Field<string>("strategy_name");
                if (strategyTable.Select($"strategy_name = '{strategyName}'").Length == 0)
                    strategyTable.Rows.Add(strategyName, false);

                DataTable t;
                if (!orderDic.TryGetValue(strategyName, out t))
                {
                    t = orderTable.Clone();
                    orderDic.Add(strategyName, t);
                }
                t.Rows.Add(row.ItemArray);
            }
        }

        //
        internal DataTable getPositionTable(string strategyName)
        {
            DataTable t;
            return positionDic.TryGetValue(strategyName, out t) ? t : new DataTable();
        }
        internal void setPosition(string strategyName,string instrumentID,int position)
        {
            try
            {
                //修改数据库
                string sql = @"replace into [t_position]
                            ([strategy_name], [instrument_id], [position], [last_time])
                            values
                            (@strategyName,@instrumentID,@position,@lastTime)";
                SQLiteHelper.ExecuteNonQuery(sql
                    , new System.Data.SQLite.SQLiteParameter("strategyName",strategyName)
                    , new System.Data.SQLite.SQLiteParameter("instrumentID",instrumentID)
                    , new System.Data.SQLite.SQLiteParameter("position",position)
                    , new System.Data.SQLite.SQLiteParameter("lastTime",DateTime.Now));

                //修改缓存
                DataTable t;
                if (positionDic.TryGetValue(strategyName, out t))
                {
                    DataRow[] rows = t.Select($"strategy_name = '{strategyName}' and instrument_id = '{instrumentID}'");
                    if (rows.Length > 0)
                    {
                        rows[0]["position"] = position;
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        internal DataTable getOrderTable(string strategyName)
        {
            DataTable t;
            return orderDic.TryGetValue(strategyName, out t) ? t : new DataTable();
        }

    }
}
