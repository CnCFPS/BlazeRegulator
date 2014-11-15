// -----------------------------------------------------------------------------
//  <copyright file="ICChatFilter.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace IrcRelay.Commands
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Atlantis.Linq;
    using Atlantis.Net.Irc;
    using Atlantis.Net.Irc.Linq;
    using BlazeRegulator.Core.Commands;

    public class ICChatFilter : ChatCommandFilter
    {
        private readonly IrcClient _client;

        public ICChatFilter(IrcClient client)
        {
            _client = client;
        }

        #region Helper Methods

        private static bool HasPermission(String sLevel, char prefix)
        {
            int level = 0;
            if (!Int32.TryParse(sLevel, out level))
            {
                return false;
            }

            var perm = CommandManager.Instance.FindPermission(level);
            if (perm == null)
            {
                return false;
            }

            var access = PrefixToString(prefix);
            return perm.UserGroups.Any(x => x.EqualsIgnoreCase(access));
        }

        private static String PrefixToString(char prefix)
        {
            switch (prefix)
            {
                case '~':
                    return "irc_founder";
                case '&':
                    return "irc_protected";
                case '@':
                    return "irc_op";
                case '%':
                    return "irc_halfop";
                case '+':
                    return "irc_voice";
                default:
                    return "irc_normal";
            }
        }

        private static String TranslatePrefixString(String str)
        {
            switch (str)
            {
                case "irc_founder":
                    return "IRC Founder Users";
                case "irc_protected":
                    return "Protected IRC Users";
                case "irc_op":
                    return "IRC Operators";
                case "irc_halfop":
                    return "IRC Half Operators";
                case "irc_voice":
                    return "Voiced IRC Users";
                default:
                    return "Normal IRC Users";
            }
        }

        #endregion

        #region Event Handlers

        private void OnPrivmsg(object sender, MessageReceivedEventArgs e)
        {
            if (e.IsChannel)
            {
                String sourceNick = e.Source.GetNickFromSource();

                var channel = _client.GetChannel(e.Target);
                PrefixList plist;
                channel.Users.TryGetValue(sourceNick, out plist);

                char prefix = '\0';
                if (plist != null)
                {
                    prefix = plist.HighestPrefix;
                }

                var sCommand = e.Message.Split(' ')[0];
                if (sCommand[0] == CommandManager.Instance.CommandPrefix)
                {
                    sCommand = sCommand.Substring(1); // remove trigger prefix.
                    var cmd = CommandManager.Instance.FindCommand(sCommand);
                    var source = CreateCommandSource(sourceNick, e.Target);

                    if (cmd != null && cmd.Enabled && cmd.CanExecuteInIRC)
                    {
                        if (HasPermission(cmd.Permission, prefix))
                        {
                            var parameters = "";
                            var msglen = e.Message.Split(' ').Length;
                            if (msglen > 1)
                            {
                                parameters = e.Message.Substring(e.Message.IndexOf(' ') + 1);
                            }

                            OnCommand(source, cmd, parameters);
                        }
                        else
                        { // Access denied.
                            source.Respond(ReplyType.Public, "{0} is not available for {1}",
                                cmd.Name,
                                TranslatePrefixString(PrefixToString(prefix)));
                        }
                    }
                    else if (cmd != null && !cmd.CanExecuteInIRC)
                    {
                        source.Respond(ReplyType.Public, "{0} cannot be executed via IRC.", cmd.Name);
                    }
                }
            }
        }

        #endregion
        
        #region Overrides of ChatCommandFilter

        public override void Initialize()
        {
            _client.PrivmsgReceivedEvent += OnPrivmsg;
        }

        protected override CommandSource CreateCommandSource(string sourceName, object data)
        {
            Debug.Assert(data is string, "Non-string value passed as data field when creating command source.");
            return new IrcSource(_client, sourceName, (string)data);
        }

        protected override void OnCommand(CommandSource source, Command command, string parameters)
        {
            var cmd = Commands.SingleOrDefault(x => x.Name.EqualsIgnoreCase(command.Name));
            if (cmd != null)
            {
                var paramLength = parameters.Split(' ').Length;
                if (paramLength >= cmd.Parameters)
                {
                    cmd.Handle(source, parameters);
                }
                else
                {
                    source.Respond(ReplyType.Public, CommandResponse.NotEnoughParams);
                }
            }
        }

        #endregion
    }
}
