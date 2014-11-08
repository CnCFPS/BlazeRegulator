// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;

	public static class Bot
	{
		private static readonly PluginManager plugins = new PluginManager();

		public static PluginManager Plugins
		{
			get { return plugins; }
		}

		/// <summary>
		/// Gets a string value representing the version of the bot.
		/// </summary>
		public static String Version
		{
			get { return "2.0"; }
		}

	    public static int VersionInt32
	    { // 2.0.0.0 => 2000
	        get { return 2000; }
	    }
	}
}
