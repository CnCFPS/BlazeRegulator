// -----------------------------------------------------------------------------
//  <copyright file="IrcPlugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace IrcRelay
{
    using BlazeRegulator.Core.Extensibility;
    using BlazeRegulator.Core.Net.Irc;

    public class IrcPlugin : Plugin
    {
        private IRC _irc;
        private EventRelay _relay;

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
            _irc = Get<IRC>();
            _relay = new EventRelay();
            _relay.Initialize(_irc);

            return true;
        }

        private void RegisterCommands()
        {
            /*
            //_irc.RegisterChatCommand(new ICTestHandler());
            _irc.RegisterChatCommand(new ICMessageHandler());
            _irc.RegisterChatCommand(new ICGameInfoHandler());
            _irc.RegisterChatCommand(new ICPlayerListHandler());
            _irc.RegisterChatCommand(new ICPlayerInfoHandler());*/
        }

        /// <summary>
        /// Called when the plugin unloads.
        /// </summary>
        /// <returns></returns>
        public override bool Unload()
        {
            return true;
        }

        #endregion
    }
}
