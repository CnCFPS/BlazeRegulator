// -----------------------------------------------------------------------------
//  <copyright file="ICGameInfoHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace IrcRelay.CommandHandlers
{
    using System;
    using System.Linq;
    using System.Text;
    using Atlantis.Linq;
    using BlazeRegulator.Core;
    using BlazeRegulator.Core.Commands;

    public class ICGameInfoHandler : CommandHandler
    {
        public ICGameInfoHandler() : base("gameinfo")
        {
        }

        #region Overrides of CommandHandler

        public override int Parameters
        {
            get { return 0; }
        }

        public override async void Handle(CommandSource source, string parameters)
        {
            var team0 = new StringBuilder();
            var team1 = new StringBuilder();

            var team0Players = Game.Players.Count(x => x.IsInGame && x.Team == 0);
            var team1Players = Game.Players.Count(x => x.IsInGame && x.Team == 1);

            team0.AppendFormat("{0}{1}{2}: {3} players {4} points{0}",
                (char)3,
                Game.TeamHandler.GetTeamColor(0),
                Game.TeamHandler.GetTeamName(0),
                team0Players,
                Game.Team0Points);

            team1.AppendFormat("{0}{1}{2}: {3} players {4} points{0}",
                (char)3,
                Game.TeamHandler.GetTeamColor(1),
                Game.TeamHandler.GetTeamName(1),
                team1Players,
                Game.Team1Points);

            var gameMode = !String.IsNullOrEmpty(Game.GameMode) && Game.GameMode.EqualsIgnoreCase("Westwood Online")
                ? "WOL"
                : "GameMode not set";

            source.Respond(ReplyType.Public, "{1}{0}15GameInfo:{1} ({0}06{2}{0}15){0}11 Map: {3}{0} {4} {5}",
                (char)3,
                (char)2,
                gameMode,
                Game.Map,
                team1,
                team0);
        }

        #endregion
    }
}
