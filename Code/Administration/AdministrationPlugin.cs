﻿// -----------------------------------------------------------------------------
//  <copyright file="AdministrationPlugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Administration
{
	using BlazeRegulator.Core.Extensibility;

	public class AdministrationPlugin : Plugin
	{
		#region Overrides of Plugin

		public override string Author
		{
			get { return "Genesis2001"; }
		}

		public override string Name
		{
			get { return "BlazeRegulator Administration Plugin"; }
		}

		public override string ShortName
		{
			get { return "AdminPlugin"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override PluginType Type
		{
			get { return PluginType.VENDOR; }
		}

		#endregion
	}
}
