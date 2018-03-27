using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMachineConsole.indicators
{
    public class IndicatorChaikin : IIndicator
    {
        public string getName()
        {
            return "Chaikin";
        }



        public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
        {
            try
            {


                double[] arrayresultTA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                arrayresultTA = new double[arrayPriceClose.Length];
                TicTacTec.TA.Library.Core.AdOsc(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, arrayVolume, 3, 10, out outBegidx, out outNbElement, arrayresultTA);
                double chaikin = arrayresultTA[outNbElement - 1];

                if (chaikin < 0)
                    return Operation.sell;
                if (chaikin > 0)
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
