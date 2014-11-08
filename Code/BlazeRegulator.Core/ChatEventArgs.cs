// -----------------------------------------------------------------------------
//  <copyright file="ChatEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
    using System;

    public class ChatEventArgs : EventArgs
	{
		public ChatEventArgs(String name, String message, ChatType type)
		{
			Name = name;
			Message = message;
			Type = type;
		}

		#region Properties

		public bool IsHostChat
		{
			get { return Type.HasFlag(ChatType.Host); }
		}

		public bool IsPrivateChat
		{
			get { return Type.HasFlag(ChatType.Private); }
		}

		public bool IsTeamChat
		{
			get { return Type.HasFlag(ChatType.Team); }
		}

		public String Message { get; private set; }

		public String Name { get; private set; }

		public ChatType Type { get; private set; }

		#endregion
	}

	#region External type: ChatType

	[Flags]
	public enum ChatType
	{
		None = 0,
		Public = 1,
		Team = 2,
		Private = 4,
		Host = 8,
		Unknown = 16,
	}

	#endregion
}
