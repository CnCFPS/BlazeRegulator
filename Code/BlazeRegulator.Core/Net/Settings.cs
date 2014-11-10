// -----------------------------------------------------------------------------
//  <copyright file="Settings.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;
	using System.Xml.Serialization;

	public class Settings
	{
		[XmlElement("Remote")]
		public RemoteSettings RemoteConfig { get; set; }
        
		#region Nested type: RemoteSettings

		public class RemoteSettings
		{
			[XmlElement("RenRem")]
			public RenRemSettings RenRemConfig { get; set; }

			[XmlElement("Log")]
			public RemoteLogSettings LogConfig { get; set; }
		}

		#endregion

		#region Nested type: RenRemSettings

		public class RenRemSettings
		{
			[XmlAttribute("Host")]
			public String Host { get; set; }

			[XmlAttribute("Port")]
			public int Port { get; set; }

			[XmlAttribute("Password")]
			public String Password { get; set; }
		}

		#endregion

		#region Nested type: RemoteLogSettings

		public class RemoteLogSettings
		{
			[XmlAttribute("Host")]
			public String Host { get; set; }

			[XmlAttribute("Port")]
			public int Port { get; set; }
		}

		#endregion
	}
}
