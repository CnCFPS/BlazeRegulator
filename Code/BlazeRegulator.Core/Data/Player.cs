// -----------------------------------------------------------------------------
//  <copyright file="Player.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using Atlantis.Linq;

	public class Player
	{
		/// <summary>
		/// Reserved keys for _data values.
		/// </summary>
		private static readonly String[] reservedKeys =
		{
			"uiPlayerId", "bConnectionLost", "dtJoinTime", "dtLeaveTime", "sName", "iScore",
			"iTeam", "bFirstPlayerInfo", "bIsInGame", "sSerial", "sVersion"
		};

		#region Indexers

		public object this[String property]
		{
			get
			{
				object value;
				_data.TryGetValue(property, out value);

				return value;
			}
			set
			{
				if (reservedKeys.Any(x => x.EqualsIgnoreCase(property)))
				{
					throw new ArgumentOutOfRangeException("property",
						"The specified property is reserved by the core and cannot be manipulated in this manor.");
				}

				_data[property] = value;
			}
		}

		#endregion

		#region Methods

		public T Get<T>(String property)
		{
			object value;
			_data.TryGetValue(property, out value);

			if (value == null)
			{
				return default(T);
			}

			if (typeof (T) == typeof (object))
			{
				return (T)value;
			}

			T val;
			try
			{
				val = (T)Convert.ChangeType(value, typeof (T));
			}
			catch
			{
				return default(T);
			}

			return val;
		}

		public void Set<T>(String property, T value)
		{
			if (reservedKeys.Any(x => x.EqualsIgnoreCase(property)))
			{
				throw new ArgumentOutOfRangeException("property",
					"The specified property is reserved by the core and cannot be manipulated in this manor.");
			}

			_data[property] = value;
		}

		#endregion

		#region Properties

		public bool ConnectionLost
		{
			get
			{
				object val;
				_data.TryGetValue("bConnectionLost", out val);

				return val is bool && (bool)val;
			}
			set { _data["bConnectionTime"] = value; }
		}

		/// <summary>
		///     Gets or sets a value indicating the player's ingame ID.
		/// </summary>
		public uint Id
		{
			get
			{
				object value;
				_data.TryGetValue("uiPlayerId", out value);

			    return value is uint ? (uint)value : 0;
			}
			set { _data["uiPlayerId"] = value; }
		}

	    /// <summary>
	    ///     Gets or sets a value indicating whether the player is ingame.
	    /// </summary>
	    public bool IsInGame
	    {
	        get
	        {
	            object value;
	            _data.TryGetValue("bIsInGame", out value);

	            return value is bool && (bool)value;
	        }
	        set { _data["bIsInGame"] = value; }
	    }

        /// <summary>
        /// <para>Gets a value indicating when the player joined.</para>
        /// <para>This is used to suppress the PlayerJoinedEvent being fired when the bot starts up.</para>
        /// </summary>
	    public DateTime JoinTime
	    {
	        get
	        {
                object value;
                _data.TryGetValue("dtJoinTime", out value);

                return value is DateTime ? (DateTime)value : DateTime.MinValue;
	        }
	        internal set { _data["dtJoinTime"] = value; }
	    }

	    /// <summary>
		///     Gets or sets a value indicating the time that player has left the game.
		/// </summary>
		public DateTime LeaveTime
		{
			get
			{
				object value;
				_data.TryGetValue("dtLeaveTime", out value);

				return value is DateTime ? (DateTime)value : DateTime.MinValue;
			}
	        internal set { _data["dtLeaveTime"] = value; }
		}

		/// <summary>
		///     Gets or sets a value indicating the player's name
		/// </summary>
		public String Name
		{
			get
			{
				object value;
				_data.TryGetValue("sName", out value);

				return value as String;
			}
			set { _data["sName"] = value; }
		}

		/// <summary>
		///     Gets or sets a value indicating the score of the current player.
		/// </summary>
		public int Score
		{
			get
			{
				object value;
				_data.TryGetValue("iScore", out value);

				return value is int ? (int)value : 0;
			}
			set { _data["iScore"] = value; }
		}

	    public string Serial
	    {
            get
            {
                object value;
                _data.TryGetValue("sSerial", out value);

                return value as String;
            }
	        set { _data["sSerial"] = value; }
	    }

	    /// <summary>
		///     Gets or sets a value indicating the team the player is on.
		/// </summary>
		public int Team
		{
			get
			{
				object value;
				_data.TryGetValue("iTeam", out value);

				return value is int ? (int)value : 0;
			}
			set { _data["iTeam"] = value; }
		}

        public String Version
        {
            get
            {
                object value;
                _data.TryGetValue("sVersion", out value);

                return value as String;
            }
            set { _data["sVersion"] = value; }
        }

		#endregion

		#region Internal notations

		internal bool FirstPlayerInfo
		{
			get
			{
				object value;
				_data.TryGetValue("bFirstPlayerInfo", out value);

				return value is bool && (bool)value;
			}
			set { _data["bFirstPlayerInfo"] = value; }
		}


	    #endregion

		#region Fields

		private readonly IDictionary<String, object> _data =
			new ConcurrentDictionary<String, object>(StringComparer.OrdinalIgnoreCase);

		#endregion

	    #region Overrides of Object

	    /// <summary>
	    /// Serves as a hash function for a particular type. 
	    /// </summary>
	    /// <returns>
	    /// A hash code for the current object.
	    /// </returns>
	    public override int GetHashCode()
	    {
	        return _data.GetHashCode();
	    }

	    /// <summary>
	    /// Returns a string that represents the current object.
	    /// </summary>
	    /// <returns>
	    /// A string that represents the current object.
	    /// </returns>
	    public override string ToString()
	    {
	        return Name;
	    }

	    #endregion
	}
}
