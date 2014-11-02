// -----------------------------------------------------------------------------
//  <copyright file="EnumEx.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using Atlantis.Linq;

	public static class EnumEx
	{
		public static T GetValueFromDescription<T>(String description)
		{
			var type = typeof (T);
			if (!type.IsEnum)
			{
				throw new ArgumentException("The specified type is not an enumeration.");
			}

			foreach (var field in type.GetFields())
			{
				var attr = Attribute.GetCustomAttribute(field, typeof (DescriptionAttribute), false) as DescriptionAttribute;
				if (attr == null) continue;

				if (attr.Description.EqualsIgnoreCase(description))
				{
					return (T)field.GetValue(null);
				}

				if (field.Name == description)
				{
					return (T)field.GetValue(null);
				}
			}

			return default(T);
		}

		public static String GetDescription(this Enum source)
		{
			var type = source.GetType();
			var minfo = type.GetMember(source.ToString());
			if (minfo.Length > 0)
			{
				var attrs = minfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false).Cast<DescriptionAttribute>().ToArray();

				if (attrs.Length > 0)
				{
					return attrs[0].Description;
				}
			}

			return source.ToString();
		}
	}
}
