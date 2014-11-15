// -----------------------------------------------------------------------------
//  <copyright file="DependencyResolver.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Extensibility
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a dependancy container 
    /// </summary>
    public class DependencyResolver
    {
        #region Fields

        private readonly IDictionary<Type, object> _dict = new ConcurrentDictionary<Type, object>();
        private readonly IDictionary<Type, Func<object>> _createInstances = new ConcurrentDictionary<Type, Func<object>>();

        #endregion
        
        #region Methods

        /// <summary>
        /// Registers the specified dependency for future resolving.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dep"></param>
        public void Register<T>(T dep)
        {
            if (!_dict.ContainsKey(typeof (T)))
            {
                _dict.Add(typeof (T), dep);
            }
        }

        /// <summary>
        /// Registers the specified type with the dependency resolver and allows delaying it's instantiation until it's resolved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="create"></param>
        public void Register<T>(Func<object> create)
        {
            var type = typeof (T);
            if (!_createInstances.ContainsKey(type) && !_dict.ContainsKey(type))
            {
                _createInstances.Add(type, create);

                if (!_dict.ContainsKey(type))
                {
                    _dict.Add(type, null);
                }
            }
        }

        /// <summary>
        /// Attempts to get a dependency from the resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            var type = typeof (T);

            object value;
            _dict.TryGetValue(type, out value);

            Func<object> create;
            _createInstances.TryGetValue(type, out create);
            if (value == null && create != null)
            {
                _dict[type] = (value = create());
            }

            return value == null ? default(T) : (T)value;
        }

        /// <summary>
        ///     <para>Deregisters the specified dependency from the DependencyResolver.</para>
        ///     <para>Note that this does not force garbage collection on the dep if it's been referenced elsewhere.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Unregister<T>()
        {
            var type = typeof (T);
            if (_dict.ContainsKey(type))
            {
                _dict.Remove(type);
            }

            if (_createInstances.ContainsKey(type))
            {
                _createInstances.Remove(type);
            }
        }

        #endregion
    }
}
