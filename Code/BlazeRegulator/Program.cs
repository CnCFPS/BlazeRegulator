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

	public class Program
	{
		private static Settings settings;

		public static void Main(string[] args)
		{
			settings = SettingsManager.LoadSettingsFrom<Settings>("Settings.xml");

			Log.Instance.WriteLine("Initializing IRC client...");
			IRC.Instance.Initialize(settings);

			Log.Instance.WriteLine("Connecting to IRC... {0}:{1}", settings.IrcConfig.Server, settings.IrcConfig.Port);
			IRC.Instance.Start();

			Console.CancelKeyPress += BotShutdown;
			while (true)
			{
				new EventWaitHandle(false, EventResetMode.ManualReset).WaitOne();
			}
		}

		private static void BotShutdown(object sender, ConsoleCancelEventArgs e)
		{
			Log.Instance.WriteLine("Closing IRC connection.");
			IRC.Instance.Shutdown();

			Log.Instance.WriteLine("Saving settings to Settings.xml");
			SettingsManager.SaveSettingsTo(settings, "Settings.xml");
		}
	}
}
