using MoneyMachineConsole.indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMachineConsole
{
    public class ReturnIndicator
    {
        public double rsi;
        public double mfi;
        public double cci;
        public double roc;
        public double mom;
        public double trix;

        public double macd;
        public double macdSignal;
        public double macdHistory;

        public decimal priceClose;

        public double chaikin;

        public DateTime date;

        public double volume;
        public double quoteVolume;



        public double stochRsiK;
        public double stochRsiD;

        public Operation operation;

        public int percentOperation;
    }
}
