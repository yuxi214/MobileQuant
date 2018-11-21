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
        }

        public void showStrategy(DataTable dt)
        {
            dataGridViewStrategy.DataSource = dt;
        }

        public void showPosition(DataTable dt)
        {
            dataGridViewPosition.DataSource = dt;
        }

        public void showOrder(DataTable dt)
        {
            dataGridViewOrder.DataSource = dt;
        }
    }
}
