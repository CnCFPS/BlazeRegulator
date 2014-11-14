// -----------------------------------------------------------------------------
//  <copyright file="ICMessageHandler.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BrIrc.CommandHandlers
{
    using BlazeRegulator.Core.Commands;
    using BlazeRegulator.Core.Net;

    public class ICMessageHandler : CommandHandler
    {
        public ICMessageHandler() : base("hostmsg")
        {
        }

        #region Overrides of CommandHandler

        public override int Parameters
        { // !msg <parameters>
            get { return 1; }
        }

        public override void Handle(CommandSource source, string parameters)
        {
            Remote.Message("({0}@IRC): {1}", source, parameters);
        }

        #endregion
    }
}
