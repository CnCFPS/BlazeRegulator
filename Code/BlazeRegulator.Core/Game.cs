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

	public static class Game
	{
		private static readonly SemaphoreSlim playersLock = new SemaphoreSlim(1, 1);
		private static readonly List<Player> players = new List<Player>();

		#region Properties

		public static String GameMode { get; set; }

		public static String Map { get; set; }

		public static IEnumerable<Player> Players
		{
			get { return players.AsReadOnly(); }
		}

		public static String TimeLeft { get; set; }

		#endregion
		
		#region Methods

		public static async void AddPlayer(Player p)
		{
			await playersLock.WaitAsync();

			if (!players.Any(x => x.Name.EqualsIgnoreCase(p.Name)))
			{
				players.Add(p);
			}

			playersLock.Release();
		}

		public static async Task<Player> GetPlayer(Func<Player, bool> predicate)
		{
			await playersLock.WaitAsync();

			var p = players.SingleOrDefault(predicate);
			
			playersLock.Release();
			return p;
		}

		public static async Task<IEnumerable<Player>> GetPlayers(int team = -3)
		{
			await playersLock.WaitAsync();

			var list = players.Where(x => team == -3 || x.Team == team);

			playersLock.Release();
			return list;
		}

		#endregion
	}
}
