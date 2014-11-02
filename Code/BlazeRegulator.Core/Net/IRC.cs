// -----------------------------------------------------------------------------
//  <copyright file="IRC.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Net
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Atlantis.Linq;
	using Atlantis.Net.Irc;

	// ReSharper disable once InconsistentNaming
	public class IRC
	{
		#region Singleton

		private static IRC instance;
		public static IRC Instance
		{
			get { return instance ?? (instance = new IRC()); }
		}

		#endregion

		#region Fields

		private readonly IrcClient client = new IrcClient();
		private bool initialized;
		private Settings settings;

		//private List<IICCommand> commands = new List<IICCommand>();

		#endregion
		
		#region Methods

		public async void Broadcast(String channel, String format, params object[] args)
		{
			String message = String.Format(format, args);
			var value = EnumEx.GetValueFromDescription<Settings.IrcChannelType>(channel);

			foreach (var item in settings.IrcConfig.Channels.Where(x => x.Type == value))
			{
				await client.SendNow("PRIVMSG {0} :{1}", item.Name, message);
			}
		}

		public void Initialize(Settings config)
		{
			settings = config;

			client.HostName = settings.IrcConfig.Server;
			client.Port = settings.IrcConfig.Port;
			client.Nick = settings.IrcConfig.Nick;
			client.Ident = "brnet45";
			client.RealName = String.Format("BlazeRegulator v{0}", "4.5");

			client.ConnectionEstablishedEvent += OnConnect;
			client.JoinEvent += OnJoin;
			client.PrivmsgReceivedEvent += OnPrivmsg;
			initialized = true;
		}

		public bool IsValidChannel(String channel)
		{
			return settings.IrcConfig.Channels.Any(x => x.Name.EqualsIgnoreCase(channel));
		}

		public bool IsGameChannel(String channel)
		{
			return
				settings.IrcConfig.Channels.Any(
				                                x =>
					                                x.Name.EqualsIgnoreCase(channel) &&
					                                (x.Type == Settings.IrcChannelType.Public ||
					                                 x.Type == Settings.IrcChannelType.Admin));
		}

		public void Shutdown()
		{
			client.Stop(String.Format("BlazeRegulator v{0} shutting down.", "4.5"));
		}

		public async void Start()
		{
			if (!initialized)
			{
				return;
			}

			await client.Start();
		}

		#endregion
		
		#region Event Handlers

		private async void OnConnect(object sender, EventArgs e)
		{
			foreach (var item in settings.IrcConfig.Channels)
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

			foreach (var item in settings.IrcConfig.OnConnect)
			{
				String exec = item.Execute;
				if (item.DelaySpecified)
				{
					double delay = item.Delay;
					await Task.Factory.StartNew(async () =>
					                                  {
						                                  await Task.Delay((int)delay * 1000);
						                                  client.Send(exec);
					                                  });
				}
				else
				{
					client.Send(exec);
				}
			}
		}
		
		private async void OnJoin(object sender, JoinPartEventArgs e)
		{
			if (e.IsMe)
			{
				if (!IsValidChannel(e.Channel))
				{
					client.Send("PART {0} :Not in my channel list.", e.Channel);
				}
				else if (IsGameChannel(e.Channel))
				{
					await client.SendNow("PRIVMSG {0} :BlazeRegulator 4.5 now online. Type !help for a list of commands.", e.Channel);
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

	#region External type: ColorCode

	public enum ColorCode
	{
		White = 0, /**< White */
		Black = 1, /**< Black */
		DarkBlue = 2, /**< Dark blue */
		DarkGreen = 3, /**< Dark green */
		Red = 4, /**< Red */
		DarkRed = 5, /**< Dark red */
		DarkViolet = 6, /**< Dark violet */
		Orange = 7, /**< Orange */
		Yellow = 8, /**< Yellow */
		LightGreen = 9, /**< Light green */
		Cyan = 10, /**< Cornflower blue */
		LightCyan = 11, /**< Light blue */
		Blue = 12, /**< Blue */
		Violet = 13, /**< Violet */
		DarkGray = 14, /**< Dark gray */
		LightGray = 15 /**< Light gray */
	};

	#endregion

	#region External type: ControlCode

	public enum ControlCode
	{
		Bold = 0x02, /**< Bold */
		Color = 0x03, /**< Color */
		Italic = 0x09, /**< Italic */
		StrikeThrough = 0x13, /**< Strike-Through */
		Reset = 0x0f, /**< Reset */
		Underline = 0x15, /**< Underline */
		Underline2 = 0x1f, /**< Underline */
		Reverse = 0x16 /**< Reverse */
	};

	#endregion
}
