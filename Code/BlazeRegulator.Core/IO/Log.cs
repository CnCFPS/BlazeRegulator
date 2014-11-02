// -----------------------------------------------------------------------------
//  <copyright file="Log.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.IO
{
	using System;

	public class Log
	{
		#region Singleton

		private static Log instance;

		public static Log Instance
		{
			get { return instance ?? (instance = new Log()); }
		}

		#endregion

		private static String GetDateTime()
		{
			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public void WriteLine(String format, params object[] args)
		{
			String message = String.Format(format, args);
			Console.WriteLine("{0} {1}", GetDateTime(), message);
		}

		public void Error(String format, params object[] args)
		{
			String message = String.Format(format, args);

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("{0} {1}", GetDateTime(), message);
			Console.ResetColor();
		}

		public void Warning(String format, params object[] args)
		{
			String message = String.Format(format, args);

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("{0} {1}", GetDateTime(), message);
			Console.ResetColor();
		}
	}
}
