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
	using Core.Data;
	using Core.IO;
	using Core.Net;

	public class Program
	{
		private static Settings settings;

		// ReSharper disable once FunctionNeverReturns
		public static void Main(string[] args)
		{
			Console.Title = String.Format("BlazeRegulator v{0} by Genesis2001", Bot.Version);
			settings = SettingsManager.LoadSettingsFrom<Settings>("Settings.xml");

			Game.SetTeamHandler(new RenegadeTeamHandler());

			Bot.Plugins.LoadDirectory("Plugins");
			
			/*
			//Game.Events.TestEvent += (s, e) => Console.WriteLine("[RECV] {0}", e.Message);
			Game.Events.ChatEvent +=
				(s, e) =>
					Console.WriteLine("[CHAT] (IsTeamChat={2},IsHostChat={3}) {0}: {1}", e.Name, e.Message, e.IsTeamChat, e.IsHostChat);
			*/

//			IRC.Instance.Initialize(settings);
//			IRC.Instance.Start();

			MainLogHandler.Instance.Initialize(settings);
			MainLogHandler.Instance.Start();

			Thread.Sleep(1000);

			Remote.Initialize(settings);
			Remote.BotMessage("BlazeRegulator {0} starting up. Type !help for a list of commands.", Bot.Version);

			Console.CancelKeyPress += BotShutdown;
			while (true)
			{
				new EventWaitHandle(false, EventResetMode.ManualReset).WaitOne();
			}
		}

		private static void BotShutdown(object sender, ConsoleCancelEventArgs e)
		{
			Remote.BotMessage("BlazeRegulator is restarting. Be good while it's gone.");
		    Bot.Plugins.UnloadAll();
			SettingsManager.SaveSettingsTo(settings, "Settings.xml");
			//IRC.Instance.Shutdown();
		}
	}
}
