// -----------------------------------------------------------------------------
//  <copyright file="IrcChannel.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc
{
    using System;
    using System.Xml.Serialization;

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
}
