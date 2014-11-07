// -----------------------------------------------------------------------------
//  <copyright file="ITeamHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
	public interface ITeamHandler
	{
		int GetTeamId(string team);

		string GetTeamName(int team);

		int GetTeamColor(int team);

		string GetIrcFormattedTeamString(int team);

		string GetIrcFormattedPlayerString(Player p);
	}
}
