// -----------------------------------------------------------------------------
//  <copyright file="IrcSource.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace IrcRelay.Commands
{
    using System;
    using Atlantis.Net.Irc;
    using BlazeRegulator.Core.Commands;

    public class IrcSource : CommandSource
    {
        private readonly IrcClient _client;
        private readonly String _channel;

        public IrcSource(IrcClient client, String source, String channel) : base(source)
        {
            _client = client;
            _channel = channel;
        }

        #region Overrides of CommandSource
        
        public override async void Respond(ReplyType reply, String format, params object[] args)
        {
            var message = String.Format(format, args);
            if (reply == ReplyType.Private)
            {
                await _client.Send("NOTICE {0} :{1}", _source, message);
            }
            else
            {
                await _client.Send("PRIVMSG {0} :{1}", _channel, message);
            }
        }

        #endregion
    }
}
