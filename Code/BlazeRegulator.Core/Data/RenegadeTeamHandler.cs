// -----------------------------------------------------------------------------
//  <copyright file="RenegadeTeamHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
	using System;
	using Net;

	public class RenegadeTeamHandler : ITeamHandler
	{
		#region Implementation of ITeamHandler

		public virtual int GetTeamId(string team)
		{
			switch (team)
			{
				case "Civilian":
					return -2;
				case "Neutral":
					return -1;
				case "Nod":
					return 0;
				case "GDI":
					return 1;
				case "Mutant":
					return 2;
				default:
					return 0;
			}
		}

		public virtual string GetTeamName(int team)
		{
			switch (team)
			{
				case -2:
					return "Civilian";
				case -1:
					return "Neutral";
				case 0:
					return "Nod";
				case 1:
					return "GDI";
				case 2:
					return "Mutant";
				default:
					return "Unknown";
			}
		}

		public virtual int GetTeamColor(int team)
		{
			switch (team)
			{
				case 0:
					return (int)ColorCode.Red;

				case 1:
					return (int)ColorCode.Yellow;

				default:
					return (int)ColorCode.LightGray;
			}
		}

		public string GetIrcFormattedTeamString(int team)
		{
		    return String.Format("{0}{1}{2}{0}", (char)(int)ControlCode.Color, GetTeamColor(team), GetTeamName(team));
		}

		public string GetIrcFormattedPlayerString(Player p)
		{
		    return String.Format("{0}{1}{2}{0}", (char)(int)ControlCode.Color, GetTeamColor(p.Team), p);
		}

		#endregion
	}
}
