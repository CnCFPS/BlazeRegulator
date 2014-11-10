// -----------------------------------------------------------------------------
//  <copyright file="ICTestCommands.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc.Commands
{
    using System;
    using BlazeRegulator.Core.Commands;

    public class ICTestHandler : CommandHandler
    {
        public ICTestHandler() : base("test")
        {
        }

        #region Overrides of CommandHandler

        public override int Parameters
        {
            get { return 0; }
        }

        public override void Handle(CommandSource source, String parameters)
        {
            source.Respond("Test command triggered. Source: {0} - Parameters: {1}", source, parameters);
        }

        #endregion
    }
}
