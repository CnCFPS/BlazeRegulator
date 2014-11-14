// -----------------------------------------------------------------------------
//  <copyright file="Game.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Atlantis.Linq;
	using Data;

	/// <summary>
	/// Contains information relating to the game and the players associated with it.
	/// </summary>
	public static class Game
	{
		private static readonly GameEventManager events = new GameEventManager();
		private static readonly SemaphoreSlim playersLock = new SemaphoreSlim(1, 1);
		private static readonly List<Player> players = new List<Player>();

		#region Properties

		public static GameEventManager Events
		{
			get { return events; }
		}

        public static int Fps { get; set; }

		public static String GameMode { get; set; }

		public static ITeamHandler TeamHandler { get; private set; }

		public static String Map { get; set; }

		public static IEnumerable<Player> Players
		{
		    get
		    {
		        var task = playersLock.WaitAsync();
                task.Wait();

		        try
		        {
		            return players.AsReadOnly();
		        }
		        finally
		        {
		            playersLock.Release();
		        }
		    }
		}

		public static DateTime ServerLoaded { get; set; }

        public static int Team0Points { get; set; }
        
        public static int Team1Points { get; set; }

		public static String TimeLeft { get; set; }
        
	    #endregion
		
		#region Methods

		public static async Task<bool> AddPlayer(Player p)
		{
            await playersLock.WaitAsync();

			try
			{
				if (players.Any(x => x.Name.EqualsIgnoreCase(p.Name)))
				{
					return false;
				}

				players.Add(p);
				return true;
			}
			finally
			{
				playersLock.Release();
			}
		}

        public static async void ClearPlayers()
        {
            await playersLock.WaitAsync();

            try
            {
                foreach (var item in players.Where(x => x.IsInGame))
                {
                    item.IsInGame = false;
                    item.LeaveTime = DateTime.UtcNow;
                }
            }
            finally
            {
                playersLock.Release();
            }
        }

		public static async Task<Player> GetPlayer(Func<Player, bool> predicate)
		{
            await playersLock.WaitAsync();

			try
			{
				return players.SingleOrDefault(predicate);
			}
			finally
			{
				playersLock.Release();
			}
		}

		public static async Task<IEnumerable<Player>> GetPlayers(int? team = null)
		{
            await playersLock.WaitAsync();

			try
			{
			    return team == null ? players.Where(x => x.IsInGame) : players.Where(x => x.IsInGame && x.Team == team);
			}
			finally
			{
				playersLock.Release();
			}
		}

		public static void SetTeamHandler(ITeamHandler handler)
		{
			TeamHandler = handler;
		}

		#endregion
	}
}
