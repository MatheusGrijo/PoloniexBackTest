using System;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Globalization;
using MoneyMachineConsole.indicators;

namespace MoneyMachineConsole
{

    public class OrderDetails
    {
        public DateTime dateBuy;
        public DateTime dateSell;
        public decimal priceBuy;
        public decimal priceSell;
        public decimal priceStop;
        public decimal totalCoin;
        public decimal totalOrder;
        public string operation;


    }
    public class BackTest
    {

        public static ReturnIndicator GetIndicatorsUSDTBackTest(List<indicators.IIndicator> listIndicators, DataTable dt, String coin, Int64 begin = 0, Int64 end = 0, int period = 300)
        {
            try
            {
                ReturnIndicator returnIndicator = new ReturnIndicator();

                DataRow[] rows = dt.Select("date >= " + begin + " and date <= " + end);


                double[] arrayPriceClose = new double[rows.Length];
                double[] arrayPriceHigh = new double[rows.Length];
                double[] arrayPriceLow = new double[rows.Length];
                double[] arrayPriceOpen = new double[rows.Length];
                double[] arrayVolume = new double[rows.Length];
                int i = 0;
                foreach (DataRow row in rows)
                {
                    arrayPriceClose[i] = double.Parse(row["close"].ToString());
                    arrayPriceHigh[i] = double.Parse(row["high"].ToString());
                    arrayPriceLow[i] = double.Parse(row["low"].ToString());
                    arrayPriceOpen[i] = double.Parse(row["open"].ToString());
                    arrayVolume[i] = double.Parse(row["volume"].ToString());
                    i++;
                }

                returnIndicator.priceClose = decimal.Parse(arrayPriceClose[arrayPriceClose.Length - 1].ToString().Replace(".", ","));


                int pointBuy = 0;
                int pointSell = 0;
                int pointNothing = 0;

                foreach (var indicator in listIndicators)
                {
                    Operation operation = indicator.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);
                    if (operation == Operation.buy)
                        pointBuy++;
                    if (operation == Operation.sell)
                        pointSell++;
                    if (operation == Operation.nothing)
                        pointNothing++;
                }



                if (pointNothing > pointBuy && pointNothing > pointSell)
                {
                    returnIndicator.operation = Operation.nothing;
                    returnIndicator.percentOperation = (pointNothing * 100) / listIndicators.Count;
                }
                else if (pointBuy > pointNothing && pointBuy > pointSell)
                {
                    returnIndicator.operation = Operation.buy;
                    returnIndicator.percentOperation = (pointBuy * 100) / listIndicators.Count;
                }
                else if (pointSell > pointNothing && pointSell > pointBuy)
                {
                    returnIndicator.operation = Operation.sell;
                    returnIndicator.percentOperation = (pointSell * 100) / listIndicators.Count;
                }
                else
                    returnIndicator.operation = Operation.nothing;

                return returnIndicator;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void backtest(String pair, DateTime _begin, DateTime _end, decimal _total, decimal totalOrder, int stopLoss, decimal profit, string strategy)
        {
            String coin = pair;            

            DateTime begin = _begin;
            DateTime beginOriginal = begin;

            DateTime endFinal = _end;

            decimal totalUSDT = _total;

            decimal totalOrderUSDT = totalOrder;
            decimal totalOrderUSDTOriginal = totalOrderUSDT;

            DateTime end = begin.AddMinutes(2000);

            Int32 unixTimestamp = (Int32)(begin.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 unixTimestampEnd = (Int32)(endFinal.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            Logger.log("Get Poloniex candles...");
            String json = null;
            while (json == null)
            {
                json = Http.get("https://poloniex.com/public?command=returnChartData&currencyPair=" + coin + "&start=" + unixTimestamp + "&end=" + unixTimestampEnd + "&period=" + 300);
                System.Threading.Thread.Sleep(1000);
            }
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            Logger.log("OK!");

            decimal totalUSDTOriginal = totalUSDT;
            decimal totalCoin = 0;

            int buyCorrect = 0;
            int buyWrong = 0;

            List<IIndicator> lst = new List<IIndicator>();

            string[] listStrategy = strategy.Split(';');
            for (int i = 0; i < listStrategy.Length; i++)
            {
                if (listStrategy[i] == "CCI")
                    lst.Add(new IndicatorCCI());
                if (listStrategy[i] == "CHAIKIN")
                    lst.Add(new IndicatorChaikin());
                if (listStrategy[i] == "MACD")
                    lst.Add(new IndicatorMACD());
                if (listStrategy[i] == "MFI")
                    lst.Add(new IndicatorMFI());
                if (listStrategy[i] == "ROC")
                    lst.Add(new IndicatorROC());
                if (listStrategy[i] == "RSI")
                    lst.Add(new IndicatorRSI());
                if (listStrategy[i] == "STOCH")
                    lst.Add(new IndicatorSTOCH());
                if (listStrategy[i] == "STOCHRSI")
                    lst.Add(new IndicatorSTOCHRSI());
                if (listStrategy[i] == "TRIX")
                    lst.Add(new IndicatorTRIX());
                if (listStrategy[i] == "WILLR")
                    lst.Add(new IndicatorWILLR());
            }

            String descriptionStrategy = "";
            foreach (var item in listStrategy)
                descriptionStrategy += item + "_";

            Logger.prefix = coin + descriptionStrategy;

            totalUSDT = totalUSDTOriginal;
            begin = beginOriginal;
            end = begin.AddMinutes(2000);            

            Logger.log("");
            Logger.log("BEGIN");
            Logger.log("*** Strategy " + descriptionStrategy);
            Logger.log("");

            List<OrderDetails> listOrders = new List<OrderDetails>();
            List<OrderDetails> listComplete = new List<OrderDetails>();

            decimal lastPriceClose = 0;
            while (true)
            {
                try
                {
                    Int64 unixbegin = (Int64)(begin.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    Int64 unixend = (Int64)(end.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    ReturnIndicator returnIndicator = GetIndicatorsUSDTBackTest(lst, dt, coin, unixbegin, unixend, 300);
                    lastPriceClose = returnIndicator.priceClose;
                    Console.Title = descriptionStrategy + " - " + coin + "|" + end.ToString() + "|USDT " + totalUSDT + "|PRICE " + returnIndicator.priceClose + "|Orders " + listOrders.Count;

                    if (returnIndicator.operation == Operation.buy && returnIndicator.percentOperation >= 100)
                    {
                        if (returnIndicator.percentOperation < 100)
                            totalOrderUSDT = (returnIndicator.percentOperation * 100) / totalOrderUSDT;

                        if (totalUSDT > 0 && totalUSDT > totalOrderUSDT)
                        {
                            totalUSDT -= totalOrderUSDT;
                            OrderDetails order = new OrderDetails();
                            order.operation = "buy";
                            order.priceSell = ((returnIndicator.priceClose * profit) / 100) + returnIndicator.priceClose;
                            order.totalCoin = totalOrderUSDT / returnIndicator.priceClose;
                            order.priceStop = returnIndicator.priceClose - ((returnIndicator.priceClose * stopLoss) / 100); ;
                            order.dateBuy = end;
                            order.priceBuy = returnIndicator.priceClose;
                            order.totalOrder = totalOrderUSDT;
                            listOrders.Add(order);

                            totalOrderUSDT = totalOrderUSDTOriginal;

                            totalCoin += order.totalCoin;
                            end = end.AddMinutes(5);
                            Logger.log("COMPRA " + end.ToString() + "|PRICE " + returnIndicator.priceClose + "|USDT " + totalUSDT + "|COIN " + totalCoin);
                        }
                        totalOrderUSDT = totalOrderUSDTOriginal;
                    }


                    List<OrderDetails> excludeOrders = new List<OrderDetails>();

                    foreach (var order in listOrders)
                    {
                        if (order.operation == "buy")
                        {
                            if (returnIndicator.priceClose >= order.priceSell || (order.priceStop >= returnIndicator.priceClose))
                            {
                                if (order.totalCoin > 0)
                                {
                                    totalUSDT += order.totalCoin * returnIndicator.priceClose;
                                    totalCoin -= order.totalCoin;
                                    if ((order.priceStop >= returnIndicator.priceClose))
                                    {
                                        Logger.log("STOP LOSS " + end.ToString() + " | PRICE " + returnIndicator.priceClose + " | USDT " + totalUSDT + " | COIN " + totalCoin);
                                        buyWrong++;
                                        order.operation = "stop";
                                    }
                                    else
                                    {
                                        Logger.log("SELL " + end.ToString() + " | PRICE " + returnIndicator.priceClose + " | USDT " + totalUSDT + " | COIN " + totalCoin);
                                        buyCorrect++;
                                        order.operation = "sell";
                                    }

                                    excludeOrders.Add(order);

                                    order.priceSell = returnIndicator.priceClose;
                                    order.dateSell = end;
                                    listComplete.Add(order);

                                    end = end.AddMinutes(5);
                                }

                            }
                        }
                    }



                    foreach (var item in excludeOrders)
                        listOrders.Remove(item);




                    begin = begin.AddMinutes(5);
                    end = end.AddMinutes(5);
                    if (end >= endFinal)
                        break;
                }
                catch
                {

                }
            }

            foreach (var order in listOrders)
                totalUSDT += order.totalCoin * lastPriceClose;

            Logger.log("Result: " + descriptionStrategy);
            Logger.log("Trade OK " + buyCorrect);
            Logger.log("Trade NOK " + buyWrong);
            Logger.log("Trade Pend " + listOrders.Count);

            Logger.log(" = Trades =");
            foreach (var item in listComplete)
            {
                Logger.log("D. buy " + item.dateBuy.ToString().PadRight(18, ' ') + "|buy " + item.priceBuy.ToString().PadRight(18, ' ') + "|D. sell " + item.dateSell.ToString().PadRight(18, ' ') + "|sell " + item.priceSell.ToString().PadRight(18, ' ') + "|perc " + (((item.priceSell * 100) / item.priceBuy) - 100).ToString().PadRight(40, ' ') + "% |time avg " + (item.dateSell - item.dateBuy).TotalMinutes.ToString().PadRight(6, ' ') + "m |op " + item.operation);
            }
            Logger.log("Total  $ " + Convert.ToString(totalUSDT) + " | Porcentagem " + Convert.ToString(((totalUSDT * 100) / totalUSDTOriginal) - 100));
            Logger.log("END");
            Logger.log("");            
        }
    }
}

