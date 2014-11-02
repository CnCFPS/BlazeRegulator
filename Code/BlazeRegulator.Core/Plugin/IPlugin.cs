// -----------------------------------------------------------------------------
//  <copyright file="IPlugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Plugin
{
	using System;
	using System.ComponentModel.Composition;

	[InheritedExport(typeof (IPlugin))]
	public interface IPlugin
	{
		#region Plugin Properties

		String Author { get; }

		String Name { get; }

		String ShortName { get; }

		PluginType Type { get; }

		String Version { get; }

		#endregion
		
		/// <summary>
		/// 
		/// </summary>
		bool Initialize();

		/// <summary>
		/// 
		/// </summary>
		void Rehash();

		/// <summary>
		/// 
		/// </summary>
		bool Unload();
	}

	internal interface IPluginAttribute
	{
	}
}
