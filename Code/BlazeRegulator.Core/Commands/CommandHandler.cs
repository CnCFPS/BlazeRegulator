// -----------------------------------------------------------------------------
//  <copyright file="CommandHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Commands
{
    using System;

    public abstract class CommandHandler
    {
        protected readonly string _commandName;

        protected CommandHandler(String commandName)
        {
            _commandName = commandName;
        }

        public String Name
        {
            get { return _commandName; }
        }

        public abstract int Parameters { get; }

        public abstract void Handle(CommandSource source, String parameters);
    }
}
