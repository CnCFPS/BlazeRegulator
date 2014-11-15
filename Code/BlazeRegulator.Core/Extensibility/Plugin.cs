// -----------------------------------------------------------------------------
//  <copyright file="Plugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Extensibility
{
	using System;
	using System.ComponentModel.Composition;

	[InheritedExport("Plugin", typeof(Plugin))]
	public abstract class Plugin
	{
		#region Properties

		public abstract String Author { get; }

		public abstract String Name { get; }

		public abstract String ShortName { get; }

		public virtual PluginType Type
		{
			get { return PluginType.EXTRA; }
		}

		public abstract String Version { get; }

		#endregion

		#region Methods

        /// <summary>
        /// Gets an instance stored in the dependency resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    public T Get<T>() where T : class
        {
            return Bot.Dependencies.Resolve<T>();
        }

	    /// <summary>
		/// Called when the plugin loads.
		/// </summary>
		/// <returns></returns>
		public abstract bool Initialize();

		/// <summary>
		/// Called when settings get (re)loaded.
		/// </summary>
		public virtual void Rehash()
		{
		}

        /// <summary>
        /// Registers an instance of an object into this plugin's IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        protected void Set<T>(T instance) where T : class
        {
            Bot.Dependencies.Register(instance);
        }

        /// <summary>
        /// Registers a type for a future object into this plugin's IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="create"></param>
        protected void Set<T>(Func<T> create) where T : class
        {
            Bot.Dependencies.Register<T>(create);
        }

		/// <summary>
		/// Called when the plugin unloads.
		/// </summary>
		/// <returns></returns>
		public abstract bool Unload();

		#endregion
	}
}
