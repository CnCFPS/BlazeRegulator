// -----------------------------------------------------------------------------
//  <copyright file="MainLogHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.IO
{
	using System;
	using System.Diagnostics;
	using System.Text.RegularExpressions;
	using Atlantis.Linq;
	using Data;
	using Net;

	public class MainLogHandler
	{
		#region Singleton

		private static MainLogHandler instance;

		public static MainLogHandler Instance
		{
			get { return instance ?? (instance = new MainLogHandler()); }
		}

		#endregion

		private readonly RenLogClient _client;

		private String _host;
		private int _port;

		private Block block = Block.ALL;

		public MainLogHandler()
		{
			_client = new RenLogClient();
		}

		#region Methods

		public void Initialize(Settings settings)
		{
			Debug.Assert(settings.RemoteConfig.LogConfig != null, "Remote::Log information is null in the configuration.");

			_host = settings.RemoteConfig.LogConfig.Host;
			_port = settings.RemoteConfig.LogConfig.Port;

			_client.RenLogEvent += OnRenLog;
			_client.ConsoleOutputEvent += OnConsoleOut;
			_client.GameLogEvent += OnGameLog;
		}

		private void OnChat(string name, string message, bool team = false)
		{
			var value = team ? ChatType.Team : ChatType.Public;

			if (name.Equals("Host", StringComparison.Ordinal))
			{
				value |= ChatType.Host;
			}

			Game.Events.Raise(this, new ChatEventArgs(name, message, value));
		}

		private void OnConsoleOut(string line)
		{
		}

		private void OnGameLog(String line)
		{
            Game.Events.Raise(this, new LogEventArgs(line));

			// "CHAT;%s;%d;%ls;%ws", messageType, Player->PlayerId, Player->Get_Name(), Message
			// "HOSTCHAT;%s;%d;%s", messageType, ID, Message
			Match m;
			/*if (line.StartsWith("CHAT"))
			{
				var tokens = line.Split(';');
				var value = EnumEx.GetValueFromDescription<ChatType>(tokens[1]);
				var message = line.Substring(line.IndexOf(';', line.IndexOf(tokens[3], StringComparison.Ordinal) + tokens[3].Length));

				Game.Events.Raise(new ChatEventArgs(tokens[3], message, value));
			}
			else if (line.StartsWith("HOSTCHAT"))
			{
				var tokens = line.Split(';');
				var value = EnumEx.GetValueFromDescription<ChatType>(tokens[1]);
				var message = line.Substring(line.IndexOf(';', line.IndexOf(tokens[2], StringComparison.Ordinal) + tokens[2].Length));

				value |= ChatType.Host;

				Game.Events.Raise(new ChatEventArgs("Host", message, value));
			}
			else*/
			if (line.TryMatch(@"^Player ([^ ]+) joined the game$", out m))
			{
				OnPrePlayerJoin(m.Groups[1].Value);
			}
			else if (line.TryMatch(@"^Player ([^ ]+) left the game$", out m))
			{
				OnPlayerLeave(m.Groups[1].Value);
			}
		}

		protected virtual async void OnPrePlayerJoin(String playerName)
		{
			var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(playerName));
			if (p != null && !p.IsInGame)
			{
				p.IsInGame = true;
			}
			else if (p == null)
			{
				p = new Player {Name = playerName};
				await Game.AddPlayer(p);
			}

			// now we want to request pinfo,game_info for updating information.
			Remote.Execute("pinfo");
			Remote.Execute("game_info");
		}

		protected virtual void OnPostPlayerJoin(Player player)
		{ // Triggered after the first player info.
			Game.Events.Raise(this, new PlayerJoinEventArgs(player));
		}

		protected virtual async void OnPlayerLeave(String playerName)
		{
			var p = await Game.GetPlayer(x => x.IsInGame && x.Name.EqualsIgnoreCase(playerName));
			if (p != null)
			{
				p.IsInGame = false;
				p.LeaveTime = DateTime.Now;

				Game.Events.Raise(this, new PlayerLeaveEventArgs(p));
			}
		}

		protected virtual void OnPlayerTeamChanged(Player player, int oldTeam, int newTeam)
		{
			Game.Events.Raise(this, new PlayerTeamChangedEventArgs(player, oldTeam, newTeam));
		}

		private async void OnRenLog(String line)
		{
			Game.Events.Raise(this, new LogEventArgs(line));

			Match m;
			if (line.Matches(@"^[^ ]+:.*"))
			{
				var name = line.Substring(0, line.IndexOf(':'));
				var message = "";

				if (line.Length > line.IndexOf(':') + 1)
				{
					message = line.Substring(line.IndexOf(':') + 1).Trim();
				}

				OnChat(name, message);
			}
			else if (line.StartsWith("[Team] "))
			{
				var name = line.Substring(7, line.IndexOf(':'));
				var message = "";

				if (line.Length > line.IndexOf(':'))
				{
					message = line.Substring(line.IndexOf(':')).Trim();
				}

				OnChat(name, message, true);
			}
			else if (line.Contains("mode active since"))
			{
				block = Block.GI;
			}
			else if (block == Block.GI)
			{
				block = Block.ALL;//foo.
			}
			else if (line.StartsWith("Start PInfo output"))
			{
				block = Block.PINFO;
			}
			else if (block == Block.PINFO &&
			    line.TryMatch(
					@"^(?<id>\d+),(?<name>[^\,]+),(?<score>\d+),(?<team>\d+),(?<ping>\d+),(?<ipaddr>[^;\,]+);(?:\d+),(?<bandwidth>\d+),(\d+),(?<kills>\d+),(?<deaths>\d+),(?<credits>\d+),(?<kd>-?\d+\.\d*)$", out m))
			{
				// 1,MPFGenesis,0,1,266,98.145.1.19;14309,32,1,0,0,310,-1.000000

				var id = uint.Parse(m.Groups["id"].Value);
				var name = m.Groups["name"].Value;
				var score = Int32.Parse(m.Groups["score"].Value);
				var team = Int32.Parse(m.Groups["team"].Value);
				var ping = Int16.Parse(m.Groups["ping"].Value);
				var ipaddr = m.Groups["ipaddr"].Value;
				var bw = Int32.Parse(m.Groups["bandwidth"].Value);

				/*
				var kills = Int16.Parse(m.Groups[9].Value);
				var deaths = Int16.Parse(m.Groups[10].Value);
				var credits = Int16.Parse(m.Groups[11].Value);
				var kd = Single.Parse(m.Groups[12].Value); */

				var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(name) || x.Id == id);
				if (p == null)
				{
					p = new Player();
					await Game.AddPlayer(p);
				}

				p.Id = id;
				p.Name = name;
				p.Score = score;

				if (team != p.Team && p.FirstPlayerInfo)
				{
					OnPlayerTeamChanged(p, p.Team, team);
				}

				p.Team = team;

				if (!p.FirstPlayerInfo)
				{
					p.FirstPlayerInfo = true;
					OnPostPlayerJoin(p);
				}
			}
			else if (line.StartsWith("End PInfo output"))
			{
				block = Block.ALL;
			}

			// TODO: Pass line off to the game event log.
		}
		
		public void Start()
		{
			_client.Start(_host, _port);
		}

		public void Stop()
		{
			_client.Stop();
		}

		#endregion

		#region Nested type: Block

		public enum Block
		{
			ALL,
			PINFO,
			GI
		}

		#endregion
	}
}
