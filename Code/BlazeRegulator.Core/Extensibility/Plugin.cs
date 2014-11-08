// -----------------------------------------------------------------------------
//  <copyright file="Plugin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Extensibility
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;

	[InheritedExport("Plugin", typeof(Plugin))]
	public abstract class Plugin
	{
	    private readonly IDictionary<Type, object> registeredTypes = new ConcurrentDictionary<Type, object>();
	    private readonly IDictionary<Type, Func<object>> objectCreation = new Dictionary<Type, Func<object>>(); 

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
        /// Gets an instance stored in the local IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    public T Get<T>() where T : class
	    {
	        var type = typeof (T);
	        object value;
	        registeredTypes.TryGetValue(type, out value);

	        Func<object> create;
            if (value == null && objectCreation.TryGetValue(type, out create))
            {
                registeredTypes[type] = (value = create());
            }

	        return value as T;
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
            if (!registeredTypes.ContainsKey(typeof(T)))
            {
                registeredTypes[typeof(T)] = instance;
            }
        }

        /// <summary>
        /// Registers a type for a future object into this plugin's IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="create"></param>
        protected void Set<T>(Func<T> create) where T : class
        {
            var type = typeof(T);
            if (!registeredTypes.ContainsKey(type))
            {
                registeredTypes[type] = null;
            }

            if (create != null && !objectCreation.ContainsKey(type))
            {
                objectCreation[type] = create;
            }
        }

		/// <summary>
		/// Called when the plugin unloads.
		/// </summary>
		/// <returns></returns>
		public abstract bool Unload();

		#endregion
	}
}
