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
            Game.Events.Raise(this, new LogEventArgs("[GAMELOG] " + line));

            if (line.StartsWith("WIN", StringComparison.OrdinalIgnoreCase))
            {
                // "WIN;%ls;%line;%d;%d"
                //      TeamName;WinType(str);NodScore;GDIScore

                String[] tokens = line.Split(';');
                if (tokens.Length < 4)
                {
                    Log.Instance.Warning("WIN line incorrectly formatted: {0}", line);
                    return;
                }

                var winnerName = tokens[1];
                var winCondition = tokens[2];
                var team0Score = int.Parse(tokens[3]);
                var team1Score = int.Parse(tokens[4]);

                OnGameOver(winnerName, winCondition, team0Score, team1Score);

                /*Int32 winnerId = TeamHandler.GetTeamId(toks[0]);
                Int32 winnerColor = TeamHandler.GetTeamColor(TeamHandler.GetTeamId(toks[0]));

                String team0Score = String.Format("{0}{1}{2}\x03: {3}", IrcControl.ColorChar, TeamHandler.GetTeamColor(0), TeamHandler.GetTeamName(0), toks[3]);
                String team1Score = String.Format("{0}{1}{2}\x03: {3}", IrcControl.ColorChar, TeamHandler.GetTeamColor(1), TeamHandler.GetTeamName(1), toks[4]);*/


                /*IRC.Instance.Message(server,
                    "\x000303[GAME END]\x03 Current game ended by \x0003{0:00}{1}\x03 via \x0002{2}\x0002.",
                    winnerColor, // 0
                    toks[1], // 1
                    toks[2], // 2
                    team0Score, // 3
                    team1Score); // 4*/
            }
            else if (line.StartsWith("DAMAGED"))
            {
                // nomnom for now.
            }
            else if (line.StartsWith("EXIT") || line.StartsWith("ENTER") || line.StartsWith("CRATE") || line.StartsWith("CREATED"))
            {
            }
            else if (line.StartsWith("KILLED"))
            {
                /*
                 * "KILLED;%line;%d;%line;%d;%d;%d;%d;%d;%line;%d;%d;%d;%d;%line;%line;%line",
                 *      ObjectType,
                 *      
                 *      Victim->Get_ID(), Victim->Get_Definition().Get_Name(),
                 *          (int)VictimPos.Y, (int)VictimPos.X, (int)VictimPos.Z, (int)Commands->Get_Facing(Victim),
                 *      
                 *      Killer?Killer->Get_ID():0, Killer?Killer->Get_Definition().Get_Name():"",
                 *          (int)KillerPos.Y, (int)KillerPos.X, (int)KillerPos.Z, (int)Commands->Get_Facing(Killer),
                 *      
                 *      Get_Current_Weapon(Killer), Translation, DATranslationManager::Translate(Killer));
                 *      
                 * KILLED;SOLDIER;1500138693;CnC_GDI_Engineer_2SF;-43;5;0;176;1500135620;CnC_Nod_Technician_0;-19;-27;2;-49;CnC_Weapon_MineTimed_Player_2Max;Hotwire;Technician
                 */

                //IRC.Instance.Message(server, "\x000307[KILL LOG]\x03 {0}", line.Substring(line.IndexOf(';', 0) + 1));
            }
            else 
            {
                Game.Events.Raise(this, new UnhandledLogEventArgs("[GAMELOG] " + line));
            }

            // "CHAT;%s;%d;%ls;%ws", messageType, Player->PlayerId, Player->Get_Name(), Message
            // "HOSTCHAT;%s;%d;%s", messageType, ID, Message
        }

        protected virtual void OnGameOver(string winnerName, string winCondition, int team0Score, int team1Score)
        {
            Game.Events.Raise(this, new GameOverEventArgs(winnerName, winCondition, team0Score, team1Score));
        }

        protected virtual void OnLevelLoaded()
        {
            Game.Events.Raise(this, new LevelLoadedEventArgs(Game.Map));
        }

        protected virtual void OnLevelLoading(string mapName)
        {
            Game.Map = mapName;
            Game.Events.Raise(this, new LevelLoadingEventArgs(mapName));
        }

        protected virtual async void OnPrePlayerJoin(String playerName)
        {
            var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(playerName));
            if (p != null && !p.IsInGame)
            {
                p.FirstPlayerInfo = false; // A join message was received, so we need to eventually trigger 
            }
            else if (p == null)
            {
                p = new Player {Name = playerName};
                await Game.AddPlayer(p);
            }

            p.IsInGame = true;
            p.JoinTime = DateTime.Now;

            Remote.Execute("player_info {0}", playerName);
            Remote.Execute("game_info");
            Remote.Execute("pinfo");
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
                p.Id = 0;
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
            Game.Events.Raise(this, new LogEventArgs("[RENLOG] " + line));

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
                var name = line.Substring(7, line.IndexOf(':') - 7);
                var message = "";

                if (line.Length > line.IndexOf(':') + 1)
                {
                    message = line.Substring(line.IndexOf(':') + 1).Trim();
                }
                
                OnChat(name, message, true);
            }
            else if (line.Contains("mode active since"))
            {
                block = Block.GI;
            }
            else if (block == Block.GI)
            {
                Match p;
                if (line.TryMatch(@"(Map|Time|Fps)\s:\s(\S+)", out p))
                {
                    String what = p.Groups[1].Value;
                    String value = p.Groups[2].Value;

                    if (what.EqualsIgnoreCase("Map"))
                    {
                        Game.Map = value;
                    }
                    else if (what.EqualsIgnoreCase("Time"))
                    {
                        Game.TimeLeft = value;
                    }
                    else if (what.EqualsIgnoreCase("Fps"))
                    {
                        Game.Fps = int.Parse(value);
                    }

                    return;
                }

                if (line.TryMatch(@"(?<team>GDI|NOD)\s:\s.*players\s*(?<points>\d+)\spoints", out p))
                {
                    String team = p.Groups["team"].Value;
                    int points = Int32.Parse(p.Groups["points"].Value);

                    if (team.EqualsIgnoreCase("GDI"))
                    {
                        Game.Team1Points = points;
                    }
                    else if (team.EqualsIgnoreCase("NOD"))
                    {
                        Game.Team0Points = points;

                        block = Block.ALL;
                    }

                    return;
                }
            }
            /*
             * [22:47:41] BlazeRegulator: Unhandled log: [RENLOG] Loading level C&C_Vile_Facility_D1.mix
             * [22:47:43] BlazeRegulator: Unhandled log: [RENLOG] Load 100% complete
             * [22:47:43] BlazeRegulator: Unhandled log: [RENLOG] Level loaded OK
             * [22:47:43] BlazeRegulator: Unhandled log: [RENLOG] Level C&C_Vile_Facility_D1.mix Loaded OK
             * [22:47:43] BlazeRegulator: Unhandled log: [RENLOG] Load took 2.0 seconds. Waiting for players...
             * [22:47:43] BlazeRegulator: Unhandled log: [RENLOG] Finished waiting after 0.0 seconds. Some players are still loading.
             * [22:47:43] BlazeRegulator: Unhandled log: [RENLOG] The Current Map Number is 51
             * 
             * [13:24:43] BlazeRegulator: Unhandled log: [RENLOG] The version of player 9 is 4.100000 r6482
             * [13:24:44] BlazeRegulator: Unhandled log: [RENLOG] The serial hash of player 9 is 00000000000000000000000000000000
             */
            else if (line.TryMatch(@"The version of player (\d+) is (.+)", out m))
            {
                var id = int.Parse(m.Groups[1].Value);

                var p = await Game.GetPlayer(x => x.Id == id);
                if (p != null)
                {
                    p.Version = m.Groups[2].Value;
                }
            }
            else if (line.TryMatch(@"The serial hash of player (\d+) is (.+)", out m))
            {
                var id = int.Parse(m.Groups[1].Value);

                var p = await Game.GetPlayer(x => x.Id == id);
                if (p != null)
                {
                    p.Serial = m.Groups[2].Value;
                }
            }
            else if (line.StartsWith("Loading level"))
            {
                var map = line.Substring(13).Trim();
                OnLevelLoading(map);
            }
            else if (line.StartsWith("Level loaded OK"))
            {
                OnLevelLoaded();
            }
            // ^(\d+)\s+(.*)\s+(-?\d+)\s+(\S+)\x09(\d+)\x09([^;]+);(\d+)\s+(\d+)\s+(\S+)
            else if (line.TryMatch(@"^\s*(?<id>\d+)\s+(?<name>[^ ]+)\s+(?<score>-?\d+)\s+(?<team>\S+)\x09(?<ping>\d+)\x09(?<ipaddr>[^;]+);(\d+)\s+(?<kbps>\d+)\s+(?<time>\S+)", out m))
            {
                /* 
                 * [10:23:58] MPFStats: debug: Id  Name           Score Side	Ping	Address               Kbits/s Time
                 * [10:23:58] MPFStats: debug:   1 Ivica007       517   NOD	203	84.112.157.213;1351   96      000.12.59
                 * [10:23:58] MPFStats: debug:   2 MPFdblaney1    624   NOD	70	173.63.200.98;57918   118     000.03.42
                 * [10:23:58] MPFStats: debug:   3 bobygto        776   GDI	67	96.22.51.125;3677     73      000.10.23
                 * [10:23:58] MPFStats: debug:   4 XD_ERROR_XD    5360  NOD	226	86.80.226.29;54726    76      000.07.16
                 * [10:23:58] MPFStats: debug:   6 Mattix1        95    GDI	178	93.200.174.174;53779  75      000.05.19
                 * [10:23:58] MPFStats: debug:   7 Acadien        394   NOD	170	89.89.46.170;60623    92      000.05.11
                 * [10:23:58] MPFStats: debug:   8 kilojay        1279  GDI	71	68.45.249.34;4010     71      000.27.15
                 * [10:23:58] MPFStats: debug:  10 Lemmy          3944  NOD	178	89.112.40.160;4565    70      000.23.04
                 * [10:23:58] MPFStats: debug:  11 tj65812        0     GDI	0	88.153.7.71;1590      0       000.00.06
                 * [10:24:23] MPFStats: debug:  12 ECgreene       1798  GDI	202	217.44.21.28;61744    89      000.30.17
                 */

                var id = int.Parse(m.Groups["id"].Value);
                var name = m.Groups["name"].Value;
                var score = int.Parse(m.Groups["score"].Value);
                var sTeam = m.Groups["team"].Value;
                var team = Game.TeamHandler.GetTeamId(sTeam);
                var ping = short.Parse(m.Groups["ping"].Value);
                //var ipaddr = m.Groups["ipaddr"].Value;
                var kbps = int.Parse(m.Groups["kbps"].Value);
                var timeLeft = m.Groups["time"].Value;

                var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(name));
                if (p != null)
                {
                    p.Id = (uint)id;
                }
                else
                {
                    p = new Player {Name = name};
                    await Game.AddPlayer(p);
                }

                p.IsInGame = true;
                p.Team = team;
                p.Score = score;

                if (!p.FirstPlayerInfo && p.JoinTime > DateTime.MinValue)
                {
                    p.FirstPlayerInfo = true;
                    OnPostPlayerJoin(p);
                }
            }
            else if (line.StartsWith("Start PInfo output"))
            {
                block = Block.PINFO;
            }
            else if (line.StartsWith("End PInfo output"))
            {
                block = Block.ALL;
            }
            else if (block == Block.PINFO &&
                line.TryMatch(@"^(?<id>\d+),(?<name>[^\,]+),(?<score>\d+),(?<team>\d+),(?<ping>\d+),(?<ipaddr>[^;\,]+);(?:\d+),(?<bandwidth>\d+),(\d+),(?<kills>\d+),(?<deaths>\d+),(?<credits>\d+),(?<kd>-?\d+\.\d*)$", out m))
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

                var p = await Game.GetPlayer(x => x.Name.EqualsIgnoreCase(name));
                if (p == null)
                {
                    p = new Player();
                    await Game.AddPlayer(p);
                }

                p.Id = id;
                p.Name = name;
                p.Score = score;
                p.IsInGame = true;

                if (team != p.Team && p.FirstPlayerInfo)
                {
                    OnPlayerTeamChanged(p, p.Team, team);
                }

                p.Team = team;
            }
            else if (line.StartsWith("Available Game Definitions", StringComparison.OrdinalIgnoreCase))
            {
                block = Block.GAMEDEFS;
            }
            else if (block == Block.GAMEDEFS)
            {
            }
            else if (block == Block.GAMEDEFS && String.IsNullOrEmpty(line))
            {
                block = Block.ALL;
            }
            else if (line.StartsWith("No players"))
            {
                Game.ClearPlayers();
            }
            else if (line.TryMatch(@"^Player ([^ ]+) joined the game$", out m))
            {
                OnPrePlayerJoin(m.Groups[1].Value);
            }
            else if (line.TryMatch(@"^Player ([^ ]+) left the game$", out m))
            {
                OnPlayerLeave(m.Groups[1].Value);
            }
            else
            {
                Game.Events.Raise(this, new UnhandledLogEventArgs("[RENLOG] " + line));
            }
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
            GI,
            GAMEDEFS,
        }

        #endregion
    }
}
