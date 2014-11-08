// -----------------------------------------------------------------------------
//  <copyright file="SettingsManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.IO
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Serialization;

	public static class SettingsManager
	{
		private static void FileToFullUri(ref String relativeUri)
		{
		    var asm = Assembly.GetEntryAssembly();
		    var dir = Path.GetDirectoryName(asm.Location);
            
		    Debug.Assert(dir != null, "Null base directory.");
            relativeUri = Path.Combine(dir, "Config", relativeUri);
		}

		public static T LoadSettingsFrom<T>(String file)
		{
			FileToFullUri(ref file);

			var serializer = new XmlSerializer(typeof (T));
			using (Stream s = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
			    Log.Instance.WriteLine("Loading settings from: {0}", Path.GetFileName(file));
				return (T)serializer.Deserialize(s);
			}
		}

		public static void SaveSettingsTo<T>(T settings, String file)
		{
		    FileToFullUri(ref file);

			var serializer = new XmlSerializer(typeof (T));
			var xmlSettings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};

			using (Stream s = new FileStream(file, FileMode.Truncate, FileAccess.Write))
			using (XmlWriter writer = XmlWriter.Create(s, xmlSettings))
			{
			    Log.Instance.WriteLine("Saving settings to: {0}", Path.GetFileName(file));

				serializer.Serialize(writer, settings);
			}
		}
	}
}
