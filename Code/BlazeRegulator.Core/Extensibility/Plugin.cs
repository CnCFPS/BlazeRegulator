// -----------------------------------------------------------------------------
//  <copyright file="Plugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Extensibility
{
	using System;
	using System.ComponentModel.Composition;

	[InheritedExport("Plugin", typeof(Plugin))]
	public abstract class Plugin
	{
		#region Properties

		public abstract String Author { get; }

		public abstract String Name { get; }

		public abstract String ShortName { get; }

		public virtual PluginType Type
		{
			get { return PluginType.EXTRA; }
		}

		public abstract String Version { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Called when the plugin loads.
		/// </summary>
		/// <returns></returns>
		public abstract bool Initialize();

		/// <summary>
		/// Called when settings get (re)loaded.
		/// </summary>
		public virtual void Rehash()
		{
		}

		/// <summary>
		/// Called when the plugin unloads.
		/// </summary>
		/// <returns></returns>
		public abstract bool Unload();

		#endregion
	}
}
