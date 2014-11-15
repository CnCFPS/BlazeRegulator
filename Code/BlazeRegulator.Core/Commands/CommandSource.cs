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
        protected String _source;

        protected CommandSource(String source)
        {
            _source = source;
        }

        #region Methods

        /// <summary>
        /// Responds to the specified source with the specified message.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public abstract void Respond(ReplyType reply, String format, params object[] args);
        
        #endregion
        
        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return _source;
        }

        #endregion
    }
}
