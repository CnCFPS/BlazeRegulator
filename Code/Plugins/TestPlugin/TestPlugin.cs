// -----------------------------------------------------------------------------
//  <copyright file="TestPlugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestPlugin
{
	using BlazeRegulator.Core.Extensibility;
	using BlazeRegulator.Core.IO;

	public class TestPlugin : Plugin
	{
		#region Overrides of Plugin

		public override string Author
		{
			get { return "Genesis2001"; }
		}

		public override string Name
		{
			get { return "A Test Plugin for BlazeRegulator"; }
		}

		public override string ShortName
		{
			get { return "TestPlugin"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override bool Initialize()
		{
			Log.Instance.WriteLine("Test plugin loaded");

			return true;
		}

		public override bool Unload()
		{
			Log.Instance.WriteLine("Test plugin unloaded.");

			return true;
		}

		#endregion
	}
}
