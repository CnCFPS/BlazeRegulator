// -----------------------------------------------------------------------------
//  <copyright file="PluginManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Plugin
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using IO;

	public class PluginManager
	{
		#region Singleton

		private static PluginManager instance;

		public static PluginManager Instance
		{
			get { return instance ?? (instance = new PluginManager()); }
		}

		#endregion

		public PluginManager()
		{
			loaded = new Dictionary<String, DirectoryCatalog>(StringComparer.OrdinalIgnoreCase);
		}

		private readonly IDictionary<String, DirectoryCatalog> loaded;

		[ImportMany(typeof (IPlugin), AllowRecomposition = true)] private List<Lazy<IPlugin>> plugins;

		#region Methods

		public IEnumerable<IPlugin> GetPlugins(Func<IPlugin, bool> predicate)
		{
			return plugins.Where(x => x.IsValueCreated).Select(x => x.Value).Where(predicate);
		}

		/// <summary>
		/// Loads the specified plugin.
		/// </summary>
		/// <param name="plugin"></param>
		public void Load(IPlugin plugin)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException("plugin");
			}

			Log.Instance.WriteLine("Loading plugin {0} v{1} by {2}", plugin.ShortName, plugin.Version, plugin.Author);

			bool ret = plugin.Initialize();
			if (ret)
			{
				plugin.Rehash();
			}

			Log.Instance.WriteLine(ret ? "Successfully loaded plugin {0}" : "Unable to load plugin {0}", plugin.ShortName);
		}

		/// <summary>
		/// Loads the specified directory.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="searchPattern"></param>
		public void LoadDirectory(String directory, String searchPattern = "*.dll")
		{
			DirectoryCatalog catalog;
			if (loaded.TryGetValue(directory, out catalog))
			{
				catalog.Refresh();
			}
			else
			{
				catalog = new DirectoryCatalog(directory, searchPattern);
				loaded.Add(directory, catalog);
			}
		}

		/// <summary>
		/// Unloads the specified plugin.
		/// </summary>
		/// <param name="plugin"></param>
		public void Unload(IPlugin plugin)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException("plugin");
			}

			Log.Instance.WriteLine("Unloading {0}...", plugin.ShortName);

			bool ret = plugin.Unload();

			Log.Instance.WriteLine(ret ? "Successfully unloaded plugin {0}" : "Unable to unload plugin {0}", plugin.ShortName);
		}

		#endregion
	}
}
