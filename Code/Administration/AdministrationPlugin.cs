// -----------------------------------------------------------------------------
//  <copyright file="AdministrationPlugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Administration
{
	using BlazeRegulator.Core.IO;
	using BlazeRegulator.Core.Plugin;

	public class AdministrationPlugin : IPlugin
	{
		#region Implementation of IPlugin

		public string Author
		{
			get { return "Genesis2001"; }
		}

		public string Name
		{
			get { return "BlazeRegulator Administration Plugin"; }
		}

		public string ShortName
		{
			get { return "Administration"; }
		}

		public string Version
		{
			get { return "1.0"; }
		}

		public PluginType Type
		{
			get { return PluginType.VENDOR; }
		}

		public bool Initialize()
		{
			Log.Instance.WriteLine("Loaded admin plugin");

			return true;
		}

		public void Rehash()
		{
		}

		public bool Unload()
		{
			Log.Instance.WriteLine("Unloaded admin plugin");

			return true;
		}

		#endregion
	}
}
