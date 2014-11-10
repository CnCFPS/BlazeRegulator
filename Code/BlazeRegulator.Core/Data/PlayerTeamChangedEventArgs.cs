// -----------------------------------------------------------------------------
//  <copyright file="PlayerTeamChangedEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
    using System;

    public class PlayerTeamChangedEventArgs : EventArgs
	{
		public PlayerTeamChangedEventArgs(Player player, int oldTeam, int newTeam)
		{
			NewTeam = newTeam;
			OldTeam = oldTeam;
			Player = player;
		}

		public int NewTeam { get; private set; }

		public int OldTeam { get; private set; }

		public Player Player { get; private set; }
	}
}
