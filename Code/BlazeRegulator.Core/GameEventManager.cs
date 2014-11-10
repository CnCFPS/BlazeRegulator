// -----------------------------------------------------------------------------
//  <copyright file="GameEventManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Data;
	using IO;


	public class GameEventManager
	{
		internal GameEventManager()
		{
			SynchronousEvents = true;
		}

		private readonly IDictionary<Type, List<Delegate>> events = new ConcurrentDictionary<Type, List<Delegate>>();

		#region Properties

		/// <summary>
		///	Gets or sets a value indicating whether to raise the events on the main thread or to use a new task.
		/// </summary>
		public bool SynchronousEvents { get; set; }

		#endregion
		
		#region Game Events

		public event EventHandler<ChatEventArgs> ChatEvent
		{
			add { Register(value); }
			remove { Unregister(value); }
		}

		public event EventHandler<PlayerJoinEventArgs> PlayerJoinEvent
		{
			add { Register(value); }
			remove { Unregister(value); }
		}

		public event EventHandler<PlayerLeaveEventArgs> PlayerLeaveEvent
		{
			add { Register(value); }
			remove { Unregister(value); }
		}

		public event EventHandler<PlayerTeamChangedEventArgs> TeamChangedEvent
		{
			add { Register(value); }
			remove { Unregister(value); }
		}
        
        public event EventHandler<LogEventArgs> LogReceivedEvent
		{
			add { Register(value); }
			remove { Unregister(value); }
		} 

		#endregion

		#region Methods

		private List<Delegate> GetEventList<TArgs>()
		{
			List<Delegate> list;
			lock (events)
			{
				List<Delegate> temp;
				events.TryGetValue(typeof (TArgs), out temp);

				if (temp == null)
				{
					list = new List<Delegate>();
					events[typeof (TArgs)] = temp = list;
				}

				list = temp;
			}

			return list;
		}

		/// <summary>
		/// Registers the specified event with the event manager.
		/// </summary>
		/// <typeparam name="TArgs"></typeparam>
		/// <param name="handler"></param>
		public void Register<TArgs>(EventHandler<TArgs> handler)
		{
			var list = GetEventList<TArgs>();
			lock (list)
			{
				list.Add(handler);
			}
		}

		/// <summary>
		/// Unregisters the specified event with the event manager.
		/// </summary>
		/// <typeparam name="TArgs"></typeparam>
		/// <param name="handler"></param>
		public void Unregister<TArgs>(EventHandler<TArgs> handler)
		{
			var list = GetEventList<TArgs>();
			lock (list)
			{
				list.RemoveAll(x => ReferenceEquals(x, handler));
			}
		}

		/// <summary>
		/// Raises the specified event on the event manager.
		/// </summary>
		/// <typeparam name="TArgs"></typeparam>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal void Raise<TArgs>(object sender, TArgs args)
		{
			var list = GetEventList<TArgs>();
			lock (list)
			{
				foreach (var item in list.OfType<EventHandler<TArgs>>())
				{
					if (SynchronousEvents)
					{
						item.Invoke(sender, args);
					}
					else
					{
						var local = item;
						Task.Run(() => local.Invoke(sender, args));
					}
				}
			}
		}

		#endregion
	}

	#region External type: LogEventArgs

	public class LogEventArgs : EventArgs
	{
		public LogEventArgs(string message)
		{
			Message = message;
		}

		public String Message { get; private set; }
	}

	#endregion
}
