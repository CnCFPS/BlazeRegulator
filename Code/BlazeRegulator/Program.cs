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
	using Core.Commands;
	using Core.Data;
	using Core.IO;
	using Core.Net;
	using Core.Net.Irc;

    public static class Program
	{
	    private static MainLogHandler logHandler;
        private static IRC irc;
		private static Settings settings;
	    private static bool exit;

		// ReSharper disable once FunctionNeverReturns
		public static void Main(string[] args)
		{
			Console.Title = String.Format("BlazeRegulator v{0} by Genesis2001", Bot.Version);
			settings = SettingsManager.LoadSettingsFrom<Settings>("Settings.xml");

            Remote.Initialize(settings);

			CommandManager.Instance.Load();

		    irc = new IRC();
		    irc.Initialize();
            irc.Start();

		    Bot.Dependencies.Register(irc);

            // Set the team handler 
            Game.SetTeamHandler(new RenegadeTeamHandler());
            Bot.Plugins.LoadDirectory("Plugins");

            logHandler = new MainLogHandler();
		    logHandler.Initialize(settings);
            logHandler.Start();
            
			Thread.Sleep(500);
            
			Remote.BotMessage("BlazeRegulator {0} starting up. Type !help for a list of commands.", Bot.Version); 

			Console.CancelKeyPress += BotShutdown;
			while (!exit)
			{
				new EventWaitHandle(false, EventResetMode.ManualReset).WaitOne();
			}
		}

		private static void BotShutdown(object sender, ConsoleCancelEventArgs e)
		{
            Remote.BotMessage("BlazeRegulator is restarting. Be good while it's gone.");
            SettingsManager.SaveSettingsTo(settings, "Settings.xml");

            if (logHandler != null)
		    {
		        logHandler.Stop();
		    }

		    Bot.Plugins.UnloadAll();
		    Thread.Sleep(1000);

            irc.Shutdown();

		    exit = true;
		}
	}
}
