// -----------------------------------------------------------------------------
//  <copyright file="ControlCode.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
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
}
