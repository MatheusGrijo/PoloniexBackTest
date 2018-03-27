/*
 * Created by SharpDevelop.
 * User: mifus_000
 * Date: 20/05/2017
 * Time: 15:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;

namespace MoneyMachineConsole
{
	/// <summary>
	/// Description of Logger.
	/// </summary>
	public class Logger
	{
		public Logger()
		{
		}

        public static string prefix = "";

		public static void log(string value)
		{
            value = "[" + DateTime.Now.ToString() + "] - " + value;

            Console.WriteLine(value);
            string myExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
            System.IO.StreamWriter w = new StreamWriter(myExeDir+"\\log\\"+prefix+"logger.txt",true);
            w.WriteLine(value);
            w.Close();
            w.Dispose();
            w = null;
		}
	}
}
