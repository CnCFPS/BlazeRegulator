// -----------------------------------------------------------------------------
//  <copyright file="CommandManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Serialization;
    using Atlantis.Linq;
    using IO;

    public class CommandManager
    {
        #region Singleton

        private static CommandManager instance;

        public static CommandManager Instance
        {
            get { return instance ?? (instance = new CommandManager()); }
        }

        #endregion

        private readonly List<Command> _commands = new List<Command>();
        private readonly List<Permission> _perms = new List<Permission>();

        #region Properties

        public char CommandPrefix { get; private set; }

        public ReadOnlyCollection<Command> Commands
        {
            get { return _commands.AsReadOnly(); }
        }

        public ReadOnlyCollection<Permission> Permissions
        {
            get { return _perms.AsReadOnly(); }
        }

        #endregion
        
        public void Load()
        {
            var settings = SettingsManager.LoadSettingsFrom<CSettings>("Commands.xml");
            
            _commands.AddRange(settings.Commands);
            _perms.AddRange(settings.Permissions);
            
            CommandPrefix = settings.Trigger[0];
        }

        public Command FindCommand(String commandName)
        {
            return _commands.SingleOrDefault(a => a.Name.EqualsIgnoreCase(commandName)
                                                  || a.Aliases.Any(b => b.EqualsIgnoreCase(commandName)));
        }

        public Permission FindPermission(int level)
        {
            return _perms.SingleOrDefault(x => x.Level == level);
        }

        public bool IsCommandEnabled(String commandName)
        {
            var cmd = FindCommand(commandName);
            return cmd != null && cmd.Enabled;
        }

        #region Nested type: CSettings

        [XmlRoot("Commands")]
        public class CSettings
        {
            [XmlAttribute("CommandTrigger")]
            public String Trigger { get; set; }

            [XmlArray("Level")]
            [XmlArrayItem("Permission")]
            public Permission[] Permissions { get; set; }
            
            [XmlElement("Command")]
            public Command[] Commands { get; set; }
        }

        #endregion
    }

    #region External type: Command

    public class Command
    {
        [XmlAttribute("Name")]
        public String Name { get; set; }

        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        [XmlArray("Groups")]
        [XmlArrayItem("Group", Type = typeof(String))]
        public String[] Groups { get; set; }
        
        [XmlAttribute("Permission")]
        public String Permission { get; set; }

        [XmlArray("Aliases")]
        [XmlArrayItem("Alias", typeof (String))]
        public String[] Aliases { get; set; }

        [XmlIgnore]
        public bool CanExecuteInGame
        {
            get { return Groups.Any(x => x.EqualsIgnoreCase("game")); }
        }

        [XmlIgnore]
        // ReSharper disable once InconsistentNaming
        public bool CanExecuteInIRC
        {
            get { return Groups.Any(x => x.EqualsIgnoreCase("irc")); }
        }
    }

    #endregion

    #region External type: Permission

    public class Permission
    {
        [XmlAttribute("Level")]
        public int Level { get; set; }

        [XmlElement("usergroup")]
        public String[] UserGroups { get; set; }
    }

    #endregion
}
