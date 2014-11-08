// -----------------------------------------------------------------------------
//  <copyright file="IRC.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Atlantis.Linq;
	using Atlantis.Net.Irc;
	using BlazeRegulator.Core;
	using BlazeRegulator.Core.IO;

	// ReSharper disable once InconsistentNaming
	public class IRC
	{
		#region Fields

		private readonly IrcClient client = new IrcClient();
		private bool initialized;
		private IrcSettings settings;

		//private List<IICCommand> commands = new List<IICCommand>();

		#endregion
		
		#region Methods

		public async void Broadcast(String channel, String format, params object[] args)
		{
			String message = String.Format(format, args);
			var value = EnumEx.GetValueFromDescription<IrcChannelType>(channel);

			foreach (var item in settings.Channels.Where(x => value.HasFlag(x.Type)))
			{
				await client.SendNow("PRIVMSG {0} :{1}", item.Name, message);
			}
		}

		public void Initialize(IrcSettings config)
		{
			Log.Instance.WriteLine("Initializing IRC client...");
			settings = config;

			client.HostName = settings.Server;
			client.Port = settings.Port;
			client.Nick = settings.Nick;
			client.Ident = "brnet" + (Bot.VersionInt32 / 1000);
		    client.RealName = String.Format("BlazeRegulator v{0}", Bot.Version);

			client.ConnectionEstablishedEvent += OnConnect;
			client.JoinEvent += OnJoin;
			client.PrivmsgReceivedEvent += OnPrivmsg;
			initialized = true;
		}

		public bool IsValidChannel(String channel)
		{
			return settings.Channels.Any(x => x.Name.EqualsIgnoreCase(channel));
		}

		public bool IsGameChannel(String channel)
		{
		    return
		        settings.Channels.Any(
		                              x =>
		                                  x.Name.EqualsIgnoreCase(channel) &&
		                                  (x.Type == IrcChannelType.Public ||
		                                   x.Type == IrcChannelType.Admin));
		}

		public void Shutdown()
		{
			Log.Instance.WriteLine("Closing IRC connection.");
			client.Stop(String.Format("BlazeRegulator v{0} shutting down.", Bot.Version));
		}

		public async void Start()
		{
			if (!initialized)
			{
				return;
			}

			Log.Instance.WriteLine("Connecting to IRC... {0}:{1}", client.HostName, client.Port);
			await client.Start();
		}

		#endregion
		
		#region Event Handlers

		private async void OnConnect(object sender, EventArgs e)
		{
		    Log.Instance.WriteLine("Connected!");
			foreach (var item in settings.Channels)
			{
				if (item.PasswordSpecified)
				{
					await client.SendNow("JOIN {0} {1}", item.Name, item.Password);
				}
				else
				{
					await client.SendNow("JOIN {0}", item.Name);
				}
			}

			foreach (var item in settings.OnConnect)
			{
				String exec = item.Execute;
				if (item.DelaySpecified)
				{
					double delay = item.Delay;
					await Task.Factory.StartNew(async () =>
					                                  {
						                                  await Task.Delay((int)delay * 1000);
						                                  await client.SendNow(exec);
					                                  });
				}
				else
				{
					await client.SendNow(exec);
				}
			}
		}
		
		private async void OnJoin(object sender, JoinPartEventArgs e)
		{
			if (e.IsMe)
			{
				if (!IsValidChannel(e.Channel))
				{
					await client.SendNow("PART {0} :Not in my channel list.", e.Channel);
				}
				else if (IsGameChannel(e.Channel))
				{
					await client.SendNow("PRIVMSG {0} :BlazeRegulator {1} now online. Type !help for a list of commands.", e.Channel, Bot.Version);
				}
			}
		}

		private void OnPrivmsg(object sender, MessageReceivedEventArgs e)
		{
			if (e.IsChannel)
			{
				//var tokens = e.Message.Split(' ');
			}
		}

		#endregion
	}
}
