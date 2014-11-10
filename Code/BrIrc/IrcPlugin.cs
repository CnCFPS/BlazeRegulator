// -----------------------------------------------------------------------------
//  <copyright file="IrcPlugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc
{
    using BlazeRegulator.Core.Extensibility;
    using BlazeRegulator.Core.IO;
    using Commands;

    public class IrcPlugin : Plugin
    {
        private IrcSettings settings;
        private IRC irc;

        #region Properties

        public override string Author
        {
            get { return "Genesis2001"; }
        }

        public override string Name
        {
            get { return "BlazeRegulator IRC Plugin"; }
        }

        public override string ShortName
        {
            get { return "BRIRC.NET"; }
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

        #region Methods

        /// <summary>
        /// Called when the plugin loads.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize()
        {
            settings = SettingsManager.LoadSettingsFrom<IrcSettings>("IRCSettings.xml");

            var localIrc = new IRC();
            localIrc.Initialize(settings);
            localIrc.Start();

            irc = localIrc;
            Set(localIrc); // other plugins may want to use the IRC instance.

            RegisterCommands();

            EventMessenger.Instance.Initialize(localIrc);
            return true;
        }

        private void RegisterCommands()
        {
            irc.RegisterChatCommand(new ICTestHandler());
            irc.RegisterChatCommand(new ICMessageHandler());
        }

        /// <summary>
        /// Called when the plugin unloads.
        /// </summary>
        /// <returns></returns>
        public override bool Unload()
        {
            irc.Shutdown();
            return true;
        }

        #endregion
    }
}
