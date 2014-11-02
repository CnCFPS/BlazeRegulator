// -----------------------------------------------------------------------------
//  <copyright file="SettingsManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.IO
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;

	public static class SettingsManager
	{
		private static void CheckPath(ref String path)
		{
		}

		public static T LoadSettingsFrom<T>(String path)
		{
			//CheckPath(ref path);

			var serializer = new XmlSerializer(typeof (T));
			using (Stream s = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				Log.Instance.WriteLine("Loading settings from: {0}", path);
				return (T)serializer.Deserialize(s);
			}
		}

		public static void SaveSettingsTo<T>(T settings, String path)
		{
			//CheckPath(ref path);

			var serializer = new XmlSerializer(typeof (T));
			var xmlSettings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};

			using (Stream s = new FileStream(path, FileMode.Truncate, FileAccess.Write))
			using (XmlWriter writer = XmlWriter.Create(s, xmlSettings))
			{
				Log.Instance.WriteLine("Saving settings to: {0}", path);

				serializer.Serialize(writer, settings);
			}
		}
	}
}
