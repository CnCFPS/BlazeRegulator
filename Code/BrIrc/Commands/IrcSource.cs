// -----------------------------------------------------------------------------
//  <copyright file="IrcSource.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc.Commands
{
    using System;
    using Atlantis.Net.Irc;
    using BlazeRegulator.Core.Commands;

    public class IrcSource : CommandSource
    {
        private readonly IrcClient _client;
        private readonly String _channel;
        private readonly String _source;

        public IrcSource(IrcClient client, String source, String channel)
        {
            _client = client;
            _source = source;
            _channel = channel;
        }

        #region Overrides of CommandSource

        public override String Name
        {
            get { return _source; }
        }

        public override async void Respond(String format, params object[] args)
        {
            var message = String.Format(format, args);
            
            await _client.Send("PRIVMSG {0} :{1}", _channel, message);
        }

        public override async void Respond(String response)
        {
            await _client.Send("PRIVMSG {0} :{1}", _channel, response);
        }

        #endregion
    }
}
