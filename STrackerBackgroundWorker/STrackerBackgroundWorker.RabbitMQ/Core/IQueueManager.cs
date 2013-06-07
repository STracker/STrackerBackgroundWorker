// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueueManager.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Interface that defines the contract of queue's operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater.RabbitMQ.Core
{
    /// <summary>
    /// The QueueManager interface.
    /// </summary>
    public interface IQueueManager
    {
        /// <summary>
        /// The push.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        void Push(Message msg);

        /// <summary>
        /// The pull.
        /// </summary>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        Message Pull();
    }
}