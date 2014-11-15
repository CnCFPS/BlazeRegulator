// -----------------------------------------------------------------------------
//  <copyright file="EventRelay.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace IrcRelay
{
    using System;
    using Atlantis.Linq;
    using BlazeRegulator.Core;
    using BlazeRegulator.Core.Data;
    using BlazeRegulator.Core.Net.Irc;

    public class EventRelay
    {
        private IRC _irc;

        internal EventRelay()
        {
            Game.Events.UnhandledLogReceivedEvent += OnLog;

            Game.Events.PlayerJoinEvent += OnPlayerJoin;
            Game.Events.PlayerLeaveEvent += OnPlayerLeave;
            Game.Events.ChatEvent += OnChat;

            Game.Events.LevelLoadingEvent += OnLevelLoading;
            Game.Events.LevelLoadedEvent += OnLevelLoaded;

            Game.Events.GameOverEvent += OnGameOver;
        }

        private async void OnChat(object sender, ChatEventArgs e)
        {
            if (e.IsHostChat)
            {
                _irc.Broadcast("B", "{0}03Host{0}: {1}", (char)3, e.Message);
            }
            else if (e.IsTeamChat)
            {
                var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(e.Name));
                var color = 15;

                if (p != null)
                {
                    color = Game.TeamHandler.GetTeamColor(p.Team);
                }

                _irc.Broadcast("A", "{1}[{0}06TEAM{0}]:{1}{0}{2} {3}: {4}", (char)3, (char)2, color, e.Name, e.Message);
            }
            // TODO: if e.IsPrivateChat ... ???
            else
            {
                var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(e.Name));
                var name = String.Format("{0}15{1}{0}", (char)3, e.Name);

                if (p != null)
                {
                    name = Game.TeamHandler.GetIrcFormattedPlayerString(p);
                }

                _irc.Broadcast("B", "{0}: {1}", name, e.Message);
            }
        }

        private void OnGameOver(object sender, GameOverEventArgs e)
        {
            // Match ended. Nod won by building destruction. Nod score: 555 points. GDI score: 444 points.

            int winnerTeamId = Game.TeamHandler.GetTeamId(e.WinnerName);
            int loserTeamId = winnerTeamId == 0 ? 1 : 0;

            var winnerScore = String.Format("{0} score: {1} points",
                Game.TeamHandler.GetIrcFormattedTeamString(winnerTeamId),
                winnerTeamId == 0 ? e.Team0Score : e.Team1Score);

            var loserScore = String.Format("{0} score: {1} points",
                Game.TeamHandler.GetIrcFormattedTeamString(loserTeamId),
                loserTeamId == 0 ? e.Team0Score : e.Team1Score);



            _irc.Broadcast("B",
                "{0:00}[{1:00}06Game Over{1:00}]:{0:00} Match end. {2} won by {3}. {4} {5}",
                (char)2,
                (char)3,
                Game.TeamHandler.GetIrcFormattedTeamString(winnerTeamId),
                e.WinCondition,
                winnerScore,
                loserScore);
        }

        private void OnLevelLoaded(object sender, LevelLoadedEventArgs e)
        {
            _irc.Broadcast("B", "{0}07Level loaded OK{0}", (char)3);
        }

        private void OnLevelLoading(object sender, LevelLoadingEventArgs e)
        {
            _irc.Broadcast("B", "{0}07Loading level {1}{0}", (char)3, e.MapName);
        }

        private void OnLog(object sender, LogEventArgs e)
        {
            _irc.Broadcast("D", "Unhandled log: {0}", e.Message);
        }

        public void Initialize(IRC irc)
        {
            _irc = irc;
        }

        private void OnPlayerLeave(object sender, PlayerLeaveEventArgs e)
        {
            _irc.Broadcast("B",
                "{0:X4}11Player {1}{0:X4}11 left the game{0:X4}",
                (char)3,
                Game.TeamHandler.GetIrcFormattedPlayerString(e.Player));
        }

        private void OnPlayerJoin(object sender, PlayerJoinEventArgs e)
        {
            _irc.Broadcast("B",
                "{0:X4}11Player {1}{0:X4}11 joined the game{0:X4}",
                (char)3,
                Game.TeamHandler.GetIrcFormattedPlayerString(e.Player));
        }
    }
}
