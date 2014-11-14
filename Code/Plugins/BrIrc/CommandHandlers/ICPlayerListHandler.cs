// -----------------------------------------------------------------------------
//  <copyright file="ICPlayerListHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc.CommandHandlers
{
    using System.Linq;
    using System.Text;
    using BlazeRegulator.Core;
    using BlazeRegulator.Core.Commands;

    public class ICPlayerListHandler : CommandHandler
    {
        public ICPlayerListHandler() : base("playerlist")
        {
        }

        #region Overrides of CommandHandler

        public override int Parameters
        {
            get { return 0; }
        }

        public override async void Handle(CommandSource source, string parameters)
        {
            var players = Game.Players.ToArray();

            var list = new StringBuilder();
            var list2 = new StringBuilder();

            list2.AppendFormat("Total: {0}", players.Length);

            int[] teams = players.Select(x => x.Team).Distinct().ToArray();
            for (int i = 0; i < teams.Length; ++i)
            {
                int team = i;
                var teamstr = Game.TeamHandler.GetIrcFormattedTeamString(team);

                var parray = players.Where(x => x.Team == team).ToArray();
                for (int j = 0; j < parray.Length; ++j)
                {
                    if (list.Length >= 100)
                    {
                        source.Respond("{0}: {1}", teamstr, list.ToString().Trim(',', ' '));
                        list.Clear();
                    }

                    list.AppendFormat(", {0}", Game.TeamHandler.GetIrcFormattedPlayerString(parray[j]));
                    if (j + 1 == parray.Length)
                    {
                        source.Respond("{0}: {1}", teamstr, list.ToString().Trim(',', ' '));
                    }
                }

                list.Clear();
                list2.AppendFormat(" - {0}{1}{2}: {3}{0}",
                    (char)3,
                    Game.TeamHandler.GetTeamColor(team),
                    Game.TeamHandler.GetTeamName(team),
                    players.Count(x => x.Team == team));
            }

            source.Respond("{0}", list2.ToString());
        }

        #endregion
    }
}
