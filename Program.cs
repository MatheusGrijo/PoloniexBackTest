/*
 * Created by SharpDevelop.
 * User: mifus_000
 * Date: 20/05/2017
 * Time: 09:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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

namespace MoneyMachineConsole
{
    class Program
    {        
        public static void Main(string[] args)
        {
            Logger.log("Backtest v0.1");
            //USDT_BTC 2018-03-01 2018-03-27 1000 100 8 0,6 CCI;RSI;MFI
            BackTest.backtest(args[0],DateTime.Parse(args[1]), DateTime.Parse(args[2]),decimal.Parse(args[3].Replace(".",",")), decimal.Parse(args[4].Replace(".", ",")), int.Parse(args[5]), decimal.Parse(args[6].Replace(".", ",")),args[7]);
            Logger.log("Aperte ENTER para sair!");
            Console.ReadLine();
        }

    }
}



