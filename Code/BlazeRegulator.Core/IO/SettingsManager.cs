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

		public static T LoadSettingsFrom<T>(String uri)
		{
			//CheckPath(ref uri);

			var serializer = new XmlSerializer(typeof (T));
			using (Stream s = new FileStream(uri, FileMode.Open, FileAccess.Read))
			{
				Log.Instance.WriteLine("Loading settings from: {0}", uri);
				return (T)serializer.Deserialize(s);
			}
		}

		public static void SaveSettingsTo<T>(T settings, String uri)
		{
			//CheckPath(ref uri);

			var serializer = new XmlSerializer(typeof (T));
			var xmlSettings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};

			using (Stream s = new FileStream(uri, FileMode.Truncate, FileAccess.Write))
			using (XmlWriter writer = XmlWriter.Create(s, xmlSettings))
			{
				Log.Instance.WriteLine("Saving settings to: {0}", uri);

				serializer.Serialize(writer, settings);
			}
		}
	}
}
