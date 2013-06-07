// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Message object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater.RabbitMQ.Core
{
    /// <summary>
    /// The Message interface.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets the argument.
        /// </summary>
        public string Arg { get; set; }
    }
}
