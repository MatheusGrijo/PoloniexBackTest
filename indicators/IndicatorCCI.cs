using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMachineConsole.indicators
{
    public class IndicatorCCI : IIndicator
    {
        public string getName()
        {
            return "CCI";
        }



        public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
        {
            try
            {
                int outBegidx, outNbElement;
                double[] arrayresultTA = new double[arrayPriceClose.Length];
                arrayresultTA = new double[arrayPriceClose.Length];
                TicTacTec.TA.Library.Core.Cci(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, 20, out outBegidx, out outNbElement, arrayresultTA);
                double value = arrayresultTA[outNbElement - 1];
                if (value > 100)
                    return Operation.sell;
                if (value < -100)
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
