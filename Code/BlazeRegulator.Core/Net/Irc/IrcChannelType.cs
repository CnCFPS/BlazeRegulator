// -----------------------------------------------------------------------------
//  <copyright file="IrcChannelType.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Net.Irc
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Flags]
    public enum IrcChannelType
    {
        [XmlIgnore] None = 0,
        [Description("P")] [XmlEnum("P")] Public = 1,
        [Description("A")] [XmlEnum("A")] Admin = 2,
        [Description("B")] [XmlEnum("B")] Both = 3,
        [Description("M")] [XmlEnum("M")] Moderator = 4,
        [Description("G")] [XmlEnum("G")] General = 8,
        [Description("D")] [XmlEnum("D")] Debug = 16,
    }
}
