// -----------------------------------------------------------------------------
//  <copyright file="EventHelpers.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Linq
{
	using System;

    public static class EventHelpers
	{
		public static void Raise<TArg>(this Action<TArg> source, TArg arg0)
		{
			var handler = source;
			if (handler != null)
			{
				handler(arg0);
			}
		}
	}
}
