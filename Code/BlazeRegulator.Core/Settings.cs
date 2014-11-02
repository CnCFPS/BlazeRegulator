// -----------------------------------------------------------------------------
//  <copyright file="Settings.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Xml.Serialization;

	public class Settings
	{
		[XmlElement("IRC")]
		public IrcSettings IrcConfig { get; set; }

		[XmlElement("Remote")]
		public RemoteSettings RemoteConfig { get; set; }

		#region Irc Types

		#region Nested type: IrcSettings

		public class IrcSettings
		{
			public String Server { get; set; }

			public int Port { get; set; }

			public String Nick { get; set; }

			[XmlArray("Channels")]
			[XmlArrayItem("Channel", Type = typeof (IrcChannel))]
			public List<IrcChannel> Channels { get; set; }

			[XmlArray("OnConnect")]
			[XmlArrayItem("Command", Type = typeof (IrcExecutor))]
			public List<IrcExecutor> OnConnect { get; set; }

			[XmlIgnore]
			public bool OnConnectSpecified
			{
				get { return OnConnect != null && OnConnect.Count > 0; }
			}
		}

		#endregion

		#region Nested type: IrcExecutor

		public class IrcExecutor
		{
			[XmlAttribute("Execute")]
			public String Execute { get; set; }

			[XmlAttribute("Delay")]
			public double Delay { get; set; }

			[XmlIgnore]
			public bool DelaySpecified
			{
				get { return Delay > 0.0; }
			}
		}

		#endregion

		#region Nested type: IrcChannel

		public class IrcChannel
		{
			[XmlAttribute("Name")]
			public String Name { get; set; }

			[XmlAttribute("Password")]
			public String Password { get; set; }

			[XmlAttribute("Type")]
			public IrcChannelType Type { get; set; }

			[XmlIgnore]
			public bool PasswordSpecified
			{
				get { return !String.IsNullOrEmpty(Password); }
			}
		}

		#endregion

		#region Nested type: IrcChannelType

		[Flags]
		public enum IrcChannelType
		{
			[XmlIgnore] None = 0,
			[Description("P")] [XmlEnum("P")] Public = 1,
			[Description("A")] [XmlEnum("A")] Admin = 2,
			[Description("B")] [XmlEnum("B")] Both = 3,
			[Description("M")] [XmlEnum("M")] Moderator = 4,
			[Description("G")] [XmlEnum("G")] General = 8,
		}

		#endregion

		#endregion

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
