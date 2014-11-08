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
    using System.Diagnostics;
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

        [ImportMany(contractName: "Plugin", contractType: typeof (Plugin), AllowRecomposition = true)] private IEnumerable<Lazy<Plugin>> plugins;

        #region Methods

        private void Compose()
        {
            Debug.Assert(container != null, "Null container when PluginManager::Compose was called.");

            container.ComposeParts(this);
            foreach (var item in plugins)
            {
                Load(item.Value);
            }
        }

        public IEnumerable<Plugin> GetPlugins(Func<Plugin, bool> predicate)
        {
            return plugins.Where(x => x.IsValueCreated).Select(x => x.Value).Where(predicate);
        }

        private void InitializeContainer()
        {
            if (core == null)
            {
                core = new AggregateCatalog();
            }

            if (container == null)
            {
                container = new CompositionContainer(core, CompositionOptions.IsThreadSafe);
            }
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

            Log.Instance.WriteLine(ret ? "Successfully loaded: {0}" : "Unable to load: {0}", plugin.Name);
        }

        /// <summary>
        /// Loads the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="searchPattern"></param>
        public void LoadDirectory(String directory, String searchPattern = "*.dll")
        {
            InitializeContainer();

            DirectoryCatalog catalog;
            if (!loaded.TryGetValue(directory, out catalog))
            {
                catalog = new DirectoryCatalog(directory, searchPattern);
                loaded.Add(directory, catalog);
                core.Catalogs.Add(catalog);
            }

            catalog.Refresh();
            Compose();
        }

        public void UnloadAll()
        {
            var local = plugins.ToArray();
            if (local.Length == 0) return;

            foreach (var item in local.Where(x => x.IsValueCreated))
            {
                Unload(item.Value);
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
            Log.Instance.WriteLine(ret ? "Successfully unloaded: {0}" : "Unable to unload: {0}", plugin.Name);
        }

        #endregion
    }
}
