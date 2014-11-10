// -----------------------------------------------------------------------------
//  <copyright file="EventMessenger.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc
{
    using BlazeRegulator.Core;
    using BlazeRegulator.Core.Data;

    public class EventMessenger
    {
        private static EventMessenger instance;

        public static EventMessenger Instance
        {
            get { return instance ?? (instance = new EventMessenger()); }
        }

        private IRC irc;

        private EventMessenger()
        {
            Game.Events.LogReceivedEvent += OnLog;

            Game.Events.PlayerJoinEvent += OnPlayerJoin;
            Game.Events.PlayerLeaveEvent += OnPlayerLeave;
        }

        private void OnLog(object sender, LogEventArgs e)
        {
            irc.Broadcast("A", "{0}", e.Message);
        }

        public void Initialize(IRC pIrc)
        {
            irc = pIrc;
        }

        private void OnPlayerLeave(object sender, PlayerLeaveEventArgs e)
        {
            irc.Broadcast("B",
                "{0:X4}{1:00}Player {2}{0:X4}{1:00} left the game{0}",
                (char)(int)ControlCode.Color,
                (int)ColorCode.LightBlue,
                Game.TeamHandler.GetIrcFormattedPlayerString(e.Player));
        }

        private void OnPlayerJoin(object sender, PlayerJoinEventArgs e)
        {
            irc.Broadcast("B",
                "{0:X4}{1:00}Player {2}{0:X4}{1:00} joined the game{0}",
                (char)(int)ControlCode.Color,
                (int)ColorCode.LightBlue,
                Game.TeamHandler.GetIrcFormattedPlayerString(e.Player));
        }
    }
}
