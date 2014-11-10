// -----------------------------------------------------------------------------
//  <copyright file="CommandSource.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Commands
{
    using System;

    public abstract class CommandSource
    {
        public abstract String Name { get; }

        public abstract void Respond(String format, params object[] args);

        public abstract void Respond(String response);

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
