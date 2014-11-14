// -----------------------------------------------------------------------------
//  <copyright file="IrcSettings.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("IRC")]
    public class IrcSettings
    {
        public String Server { get; set; }

        public int Port { get; set; }

        public String Nick { get; set; }

        [XmlArray("Channels")]
        [XmlArrayItem("Channel", Type = typeof(IrcChannel))]
        public List<IrcChannel> Channels { get; set; }

        [XmlArray("OnConnect")]
        [XmlArrayItem("Command", Type = typeof(IrcExecutor))]
        public List<IrcExecutor> OnConnect { get; set; }

        [XmlIgnore]
        public bool OnConnectSpecified
        {
            get { return OnConnect != null && OnConnect.Count > 0; }
        }

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
    }
}
