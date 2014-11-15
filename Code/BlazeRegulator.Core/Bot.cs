// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;
	using Extensibility;

    public static class Bot
	{
		private static readonly PluginManager plugins = new PluginManager();
        private static readonly DependencyResolver container = new DependencyResolver();

        public static DependencyResolver Dependencies
        {
            get { return container; }
        }
        
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

	    public static double VersionAsDouble
	    { // 2.0.0.0 => 2000
	        get { return 2000.0; }
	    }
	}
}
