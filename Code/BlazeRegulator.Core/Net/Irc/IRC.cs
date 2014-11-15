// -----------------------------------------------------------------------------
//  <copyright file="IRC.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Net.Irc
{
    using Atlantis.Net.Irc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Atlantis.Linq;
    using Core;
    using IO;
    using Linq;

    // ReSharper disable once InconsistentNaming
	public class IRC
	{
		#region Fields
        
        private readonly IrcClient _client = new IrcClient();
		private bool initialized;
		private IrcSettings settings;

		#endregion

		#region Methods

		public async void Broadcast(String channel, String format, params object[] args)
		{
			String message = String.Format(format, args);
			var value = EnumEx.GetValueFromDescription<IrcChannelType>(channel);

#if !DEBUG
            // We don't want to broadcast to a debug channel if there we're not a debug build.
            if (value.HasFlag(IrcChannelType.Debug))
		    {
                value &= ~IrcChannelType.Debug;
		    }
#endif

            foreach (var item in settings.Channels.Where(x => value.HasFlag(x.Type)))
            {
                await _client.Send("PRIVMSG {0} :{1}", item.Name, message);
            }
		}

        /// <summary>
        /// 
        /// </summary>
		public void Initialize()
        {
            settings = SettingsManager.LoadSettingsFrom<IrcSettings>("IRCSettings.xml");

			Log.Instance.WriteLine("Initializing IRC client...");
            
			_client.HostName = settings.Server;
			_client.Port = settings.Port;
			_client.Nick = settings.Nick;
			_client.Ident = "brnet" + (Bot.VersionAsDouble / 1000);
		    _client.RealName = String.Format("BlazeRegulator v{0}", Bot.Version);

			_client.ConnectionEstablishedEvent += OnConnect;
			_client.JoinEvent += OnJoin;
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
		                                   x.Type == IrcChannelType.Admin ||
		                                   x.Type == IrcChannelType.Debug));
		}

        public void Shutdown()
		{
			Log.Instance.WriteLine("Closing IRC connection.");
			_client.Stop(String.Format("BlazeRegulator v{0} shutting down.", Bot.Version));
		}

		public async void Start()
		{
			if (!initialized)
			{
				return;
			}

			Log.Instance.WriteLine("Connecting to IRC... {0}:{1}", _client.HostName, _client.Port);
			await _client.Start();
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
					await _client.Send("JOIN {0} {1}", item.Name, item.Password);
				}
				else
				{
                    await _client.Send("JOIN {0}", item.Name);
				}
			}

			foreach (var item in settings.OnConnect)
			{
				String exec = item.Execute;
				if (item.DelaySpecified)
				{
					double delay = item.Delay;
				    await Task.Factory.StartNew(() =>
				                                {
				                                    System.Threading.Thread.Sleep((int)delay * 1000);
				                                    _client.Send(exec);
				                                });
				}
				else
				{
                    await _client.Send(exec);
				}
			}
		}
		
		private async void OnJoin(object sender, JoinPartEventArgs e)
		{
			if (e.IsMe)
			{
				if (!IsValidChannel(e.Channel))
				{
                    await _client.Send("PART {0} :Not in my channel list.", e.Channel);
				}
				else if (IsGameChannel(e.Channel))
				{
                    await _client.Send("PRIVMSG {0} :BlazeRegulator {1} now online. Type !help for a list of commands.", e.Channel, Bot.Version);
				}
			}
		}

		#endregion
	}
}
