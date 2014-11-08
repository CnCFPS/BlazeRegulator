// -----------------------------------------------------------------------------
//  <copyright file="PluginManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using IO;
    using Extensibility;

    public class PluginManager
    {
        internal PluginManager()
        {
            loaded = new Dictionary<String, DirectoryCatalog>(StringComparer.OrdinalIgnoreCase);
        }

        private CompositionContainer container;
        private AggregateCatalog core;
        private readonly IDictionary<String, DirectoryCatalog> loaded;

        [ImportMany(typeof(Plugin), AllowRecomposition = true)]
        private List<Lazy<Plugin>> plugins;

        #region Methods

        public IEnumerable<Plugin> GetPlugins(Func<Plugin, bool> predicate)
        {
            return plugins.Where(x => x.IsValueCreated).Select(x => x.Value).Where(predicate);
        }

        /// <summary>
        /// Loads the specified plugin.
        /// </summary>
        /// <param name="plugin"></param>
        public void Load(Plugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            Log.Instance.WriteLine("Loading {0} - Version {1} by {2}", plugin.Name, plugin.Version, plugin.Author);
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
            if (core == null)
            {
                core = new AggregateCatalog();
            }

            if (container == null)
            {
                container = new CompositionContainer(core, CompositionOptions.IsThreadSafe);
            }

            DirectoryCatalog catalog;
            if (!loaded.TryGetValue(directory, out catalog))
            {
                catalog = new DirectoryCatalog(directory, searchPattern);
                loaded.Add(directory, catalog);
                core.Catalogs.Add(catalog);
            }

            catalog.Refresh();
        }

        public void UnloadAll()
        {
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var item in plugins.Where(x => x.IsValueCreated).Select(x => x.Value))
                {
                    Unload(item);
                }
            }
        }

        /// <summary>
        /// Unloads the specified plugin.
        /// </summary>
        /// <param name="plugin"></param>
        public void Unload(Plugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            bool ret = plugin.Unload();
            Log.Instance.WriteLine(ret ? "Successfully unloaded plugin {0}" : "Unable to unload plugin {0}", plugin.ShortName);
        }

        #endregion
    }
}
