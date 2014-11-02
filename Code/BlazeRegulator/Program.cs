// -----------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator
{
	using System;
	using System.Threading;
	using Core;
	using Core.IO;
	using Core.Net;

	public class Program
	{
		private static Settings settings;

		// ReSharper disable once FunctionNeverReturns
		public static void Main(string[] args)
		{
			settings = SettingsManager.LoadSettingsFrom<Settings>("Settings.xml");

			Log.Instance.WriteLine("Initializing IRC client...");
			IRC.Instance.Initialize(settings);

			Log.Instance.WriteLine("Connecting to IRC... {0}:{1}", settings.IrcConfig.Server, settings.IrcConfig.Port);
			IRC.Instance.Start();

			// TODO: Initialize renlog monitoring.

			Remote.Initialize(settings);
			Remote.BotMessage("BlazeRegulator {0} starting up. Type !help for a list of commands.", "4.5");

			Console.CancelKeyPress += BotShutdown;
			while (true)
			{
				new EventWaitHandle(false, EventResetMode.ManualReset).WaitOne();
			}
		}

		private static void BotShutdown(object sender, ConsoleCancelEventArgs e)
		{
			Remote.BotMessage("BlazeRegulator {0} shutting down.", "4.5");
			Log.Instance.WriteLine("Closing IRC connection.");
			IRC.Instance.Shutdown();

			Log.Instance.WriteLine("Saving settings to Settings.xml");
			SettingsManager.SaveSettingsTo(settings, "Settings.xml");
		}
	}
}
