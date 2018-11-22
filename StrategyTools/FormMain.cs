using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrategyTools
{
    public partial class FormMain : Form
    {
        MainPresenter mPresenter;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            mPresenter = new MainPresenter(this);
            if (dataGridViewStrategy.Rows.Count > 0)
            {
                string strategyName = (string)dataGridViewStrategy.Rows[0].Cells["strategy_name"].Value;
                mPresenter.loadPosition(strategyName);
                mPresenter.loadOrder(strategyName);
            }
        }

        public void showStrategy(DataTable dt)
        {
            dataGridViewStrategy.DataSource = dt;
        }

        public void showPosition(DataTable dt)
        {
            dataGridViewPosition.DataSource = dt;
            //前两列不显示
            if(dataGridViewPosition.Rows.Count > 0)
            {
                dataGridViewPosition.Columns["id"].Visible = false;
                dataGridViewPosition.Columns["strategy_name"].Visible = false;
            }
        }

        public void showOrder(DataTable dt)
        {
            dataGridViewOrder.DataSource = dt;
            //前两列不显示
            if(dataGridViewOrder.Rows.Count > 0)
            {
                dataGridViewOrder.Columns["id"].Visible = false;
                dataGridViewOrder.Columns["strategy_name"].Visible = false;
            }
        }

        private void dataGridViewStrategy_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string strategyName = (string)dataGridViewStrategy.Rows[e.RowIndex].Cells["strategy_name"].Value;
            mPresenter.loadPosition(strategyName);
            mPresenter.loadOrder(strategyName);
        }
    }
}
