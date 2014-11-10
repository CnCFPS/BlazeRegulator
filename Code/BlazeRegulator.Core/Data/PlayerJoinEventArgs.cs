// -----------------------------------------------------------------------------
//  <copyright file="PlayerJoinEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
    using System;

    public class PlayerJoinEventArgs : EventArgs
	{
		public PlayerJoinEventArgs(Player player)
		{
			Player = player;
		}

		public Player Player { get; private set; }
	}
}
