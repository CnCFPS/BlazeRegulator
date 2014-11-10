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
	using BlazeRegulator.Core.Commands;
	using BlazeRegulator.Core.IO;
	using BlazeRegulator.Core.Linq;
	using Commands;

    // ReSharper disable once InconsistentNaming
	public class IRC
	{
	    #region Constructor(s)

	    internal IRC()
	    {
	        _filter = new ICChatFilter(client);
	    }

	    #endregion
        
		#region Fields
        
        private readonly IrcClient client = new IrcClient();
	    private readonly ICChatFilter _filter;

		private bool initialized;
		private IrcSettings settings;

		#endregion

	    #region Properties

	    //internal List<IICCommand> Commands { get; private set; }

	    #endregion
        
		#region Methods

		public async void Broadcast(String channel, String format, params object[] args)
		{
			String message = String.Format(format, args);
			var value = EnumEx.GetValueFromDescription<IrcChannelType>(channel);

			foreach (var item in settings.Channels.Where(x => value.HasFlag(x.Type)))
			{
				await client.Send("PRIVMSG {0} :{1}", item.Name, message);
			}
		}

		public void Initialize(IrcSettings config)
		{
			Log.Instance.WriteLine("Initializing IRC client...");
			settings = config;

            _filter.Initialize();

			client.HostName = settings.Server;
			client.Port = settings.Port;
			client.Nick = settings.Nick;
			client.Ident = "brnet" + (Bot.VersionInt32 / 1000);
		    client.RealName = String.Format("BlazeRegulator v{0}", Bot.Version);

			client.ConnectionEstablishedEvent += OnConnect;
			client.JoinEvent += OnJoin;
			//client.PrivmsgReceivedEvent += OnPrivmsg;
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

	    public void RegisterChatCommand<THandler>(THandler handler) where THandler : CommandHandler
	    {
	        _filter.RegisterCommand(handler);
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

	    public void UnregisterChatCommand<THandler>(THandler handler) where THandler : CommandHandler
	    {
	        _filter.UnregisterCommand(handler);
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
					await client.Send("JOIN {0} {1}", item.Name, item.Password);
				}
				else
				{
                    await client.Send("JOIN {0}", item.Name);
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
                                                          await client.Send(exec);
					                                  });
				}
				else
				{
                    await client.Send(exec);
				}
			}
		}
		
		private async void OnJoin(object sender, JoinPartEventArgs e)
		{
			if (e.IsMe)
			{
				if (!IsValidChannel(e.Channel))
				{
                    await client.Send("PART {0} :Not in my channel list.", e.Channel);
				}
				else if (IsGameChannel(e.Channel))
				{
                    await client.Send("PRIVMSG {0} :BlazeRegulator {1} now online. Type !help for a list of commands.", e.Channel, Bot.Version);
				}
			}
		}

		#endregion
	}
}
