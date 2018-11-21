using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyTools
{
    internal class MainPresenter
    {
        private FormMain mView;
        private MainModel mModel = MainModel.Instance;
        public MainPresenter(FormMain view)
        {
            mView = view;
            init();
        }

        private void init()
        {
            mView.showStrategy(mModel.StrategyTable);
        }

        public void loadPosition(string strategyName)
        {
            mView.showPosition(mModel.getPositionTable(strategyName));
        }

        public void loadOrder(string strategyName)
        {
            mView.showOrder(mModel.getOrderTable(strategyName));
        }
    }
}
