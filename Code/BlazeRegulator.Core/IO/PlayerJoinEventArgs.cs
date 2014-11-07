// -----------------------------------------------------------------------------
//  <copyright file="PlayerJoinEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.IO
{
	using System;
	using Data;

	public class PlayerJoinEventArgs : EventArgs
	{
		public PlayerJoinEventArgs(Player player)
		{
			Player = player;
		}

		public Player Player { get; private set; }
	}
}
