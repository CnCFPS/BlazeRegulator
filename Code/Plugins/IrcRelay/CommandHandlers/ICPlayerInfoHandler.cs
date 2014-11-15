// -----------------------------------------------------------------------------
//  <copyright file="ICPlayerInfoHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace IrcRelay.CommandHandlers
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Atlantis.Linq;
    using BlazeRegulator.Core;
    using BlazeRegulator.Core.Commands;
    using BlazeRegulator.Core.Net;

    public class ICPlayerInfoHandler : CommandHandler
    {
        public ICPlayerInfoHandler() : base("playerinfo")
        {
        }

        #region Overrides of CommandHandler

        public override int Parameters
        {
            get { return 1; }
        }

        public override async void Handle(CommandSource source, string parameters)
        {
            var tokens = parameters.Split(' ');

            Remote.Execute("player_info");
            Remote.Execute("pinfo");

            int id;
            if (int.TryParse(tokens[0], out id))
            {
                var p = await Game.GetPlayer(x => x.Id == id);
                if (p != null)
                {
                    source.Respond(ReplyType.Public, "[PI] ID: {0} Name: {1} Score: {3} Serial: {4} Version: {5}",
                            p.Id,
                            p.Name,
                            null, /** Ping (Group 2) */
                            p.Score,
                            p.Serial,
                            p.Version);
                }
            }
            else
            {
                if (!Game.Players.Any())
                {
                    source.Respond(ReplyType.Public, "No players.");
                    return;
                }

                var regex = "^" + Regex.Escape(tokens[0]).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
                var players = (await Game.GetPlayers(x => x.Name.Matches(regex))).ToArray();

                if (players.Length > 10)
                {
                    // source.Respond(PRIVATE, "...");
                }
                else if (players.Length == 0)
                {
                    source.Respond(ReplyType.Public, "No players matching the specified criteria.");
                }
                else
                {
                    foreach (var item in players)
                    {
                        /*
                         * ID[" + plr.id + "]
                         * Name[" + plr.getColoredName() + "10]
                         * Ping[" + plr.getColoredPing() + "10]
                         * Score[" + plr.score + "]
                         * IPAddress[" + plr.addr.getHostAddress() + "]
                         * SerialHash[" + (plr.serial.equals("") ? "Unknown" : plr.serial) + "]" + "
                         * Bandwidth[" + plr.lastBandwidth + "kB/s]
                         * KD[" + plr.kills + "k/" + plr.deaths + "d=" + plr.getKD() + ")]
                         * Credits[" + plr.credits + "]
                         * Version[" + (plr.version == 0.0 ? "Unknown" : plr.version) + "]");
                         */

                        source.Respond(ReplyType.Public, "[PI] ID: {0} Name: {1} Score: {3} Serial: {4} Version: {5}",
                            item.Id,
                            item.Name,
                            null, /** Ping (Group 2) */
                            item.Score,
                            item.Serial,
                            item.Version);
                    }
                }
            }
        }

        #endregion
    }
}
