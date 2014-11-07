// -----------------------------------------------------------------------------
//  <copyright file="RenRemComm.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Net
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// High-level class representing the Renegade remote console.
	/// </summary>
	public static class Remote
	{
		private static bool initialized;
		private static RenRemComm comm;
		
		/// <summary>
		/// Sends the specified formatted hostmsg to the server with the bot's prefix prepended to the message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void BotMessage(String format, params object[] args)
		{
			if (!initialized) return;

			var message = String.Format(format, args);
			Execute("msg [BR] {0}", message);
		}

		/// <summary>
		/// Executes the specified command on the server.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void Execute(String format, params object[] args)
		{
			if (!initialized) return;

			comm.Send(format, args);
		}

		/// <summary>
		/// Initializes the RenRem communication protocol for Renegade.
		/// </summary>
		/// <param name="settings"></param>
		public static void Initialize(Settings settings)
		{
			if (initialized || comm != null) return;

			Debug.Assert(settings.RemoteConfig != null, "No Remote configuration found!");
			Debug.Assert(settings.RemoteConfig.RenRemConfig != null, "No RenRem configuration found!");

			var info = settings.RemoteConfig.RenRemConfig;
			comm = new RenRemComm(info.Host, info.Port, info.Password, new TimeSpan(0, 0, 2, 0));

			initialized = true;
		}

		/// <summary>
		/// Sends the specified formatted hostmsg to the server.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void Message(String format, params object[] args)
		{
			if (!initialized) return;

			var message = String.Format(format, args);
			Execute("msg {0}", message);
		}
	}
}
