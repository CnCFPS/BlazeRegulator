// -----------------------------------------------------------------------------
//  <copyright file="ChatCommandFilter.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Atlantis.Linq;

    public abstract class ChatCommandFilter
    {
        private readonly List<CommandHandler> _commands = new List<CommandHandler>();

        public ReadOnlyCollection<CommandHandler> Commands
        {
            get { return _commands.AsReadOnly(); }
        }

        /// <summary>
        /// Registers the specified command handler with the parser.
        /// </summary>
        /// <typeparam name="TCommandHandler"></typeparam>
        /// <param name="handler"></param>
        public void RegisterCommand<TCommandHandler>(TCommandHandler handler) where TCommandHandler : CommandHandler
        {
            lock (_commands)
            {
                if (!_commands.Any(x => x.Name.EqualsIgnoreCase(handler.Name)))
                {
                    _commands.Add(handler);
                }
            }
        }

        /// <summary>
        /// Unregisters the specified command handler.
        /// </summary>
        /// <typeparam name="TCommandHandler"></typeparam>
        /// <param name="handler"></param>
        public void UnregisterCommand<TCommandHandler>(TCommandHandler handler) where TCommandHandler : CommandHandler
        {
            lock (_commands)
            {
                _commands.RemoveAll(x => x == handler);
            }
        }

        public virtual void Initialize()
        {
            
        }

        protected abstract CommandSource CreateCommandSource(String sourceName, object data);

        protected virtual void OnCommand(CommandSource source, Command command, String parameters)
        {
            OnCommand(source, command.Name, parameters);
        }

        protected virtual void OnCommand(CommandSource source, String command, String parmaeters)
        {
            
        }
    }
}
