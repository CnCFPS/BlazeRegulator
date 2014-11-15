// -----------------------------------------------------------------------------
//  <copyright file="APathBeyondTeamHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
    // TODO: This class should be moved to a new plugin called "APB" or something.

    public class APathBeyondTeamHandler : RenegadeTeamHandler
	{
		#region Overrides of RenegadeTeamHandler

		public override int GetTeamId(string team)
		{
			switch (team)
			{
				case "Sov":
				case "Soviets":
				{
					return 0;
				}
				case "All":
				case "Allies":
				{
					return 1;
				}
				default:
					return base.GetTeamId(team);
			}
		}

		public override int GetTeamColor(int team)
		{
			switch (team)
			{
				case 0:
			        return (int)ColorCode.Red;

				case 1:
			        return (int)ColorCode.Teal;

				default:
			        return (int)ColorCode.LightGray;
			}
		}

		public override string GetTeamName(int team)
		{
			switch (team)
			{
				case 0:
					return "Soviets";
				case 1:
					return "Allies";
				default:
					return base.GetTeamName(team);
			}
		}

		#endregion
	}
}
