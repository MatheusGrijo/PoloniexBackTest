using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMachineConsole.indicators
{
    public class IndicatorMACD : IIndicator
    {
        public string getName()
        {
            return "MACD";
        }



        public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
        {
            try
            {


                double[] arrayresultTA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                double[] macdSignal = new double[arrayPriceClose.Length];
                double[] macdHist = new double[arrayPriceClose.Length];
                TicTacTec.TA.Library.Core.Macd(0, arrayPriceClose.Length - 1, arrayPriceClose, 20, 50, 9, out outBegidx, out outNbElement, arrayresultTA, macdSignal, macdHist);
                double macd = arrayresultTA[outNbElement - 1];
                double signal = macdSignal[outNbElement - 1];
                double macdHistory = macdHist[outNbElement - 1];

                if ((macd- signal) < 0)
                    return Operation.sell;
                if ((macd - signal) > 0)
                    return Operation.buy;

                return Operation.nothing;
            }
            catch
            {
                return Operation.nothing;
            }
        }
    }
}
