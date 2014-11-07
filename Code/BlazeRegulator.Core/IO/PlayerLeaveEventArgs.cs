// -----------------------------------------------------------------------------
//  <copyright file="PlayerLeaveEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.IO
{
	using Data;

	public class PlayerLeaveEventArgs : PlayerJoinEventArgs
	{
		public PlayerLeaveEventArgs(Player player) : base(player)
		{
		}
	}
}
