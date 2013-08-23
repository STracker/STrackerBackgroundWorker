// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Entry point for STrackerUpdater.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker
{
    using System;

    using global::Ninject;

    using STrackerBackgroundWorker.Ninject;
    using STrackerBackgroundWorker.RabbitMQ;

    using STrackerServer.Logger.Core;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        public static void Main()
        {
            QueueManager queueM;
            ILogger logger;

            using (IKernel kernel = new StandardKernel(new Module()))
            {
                queueM = kernel.Get<QueueManager>();
                logger = kernel.Get<ILogger>();
            }

            var container = new CommandContainer();

            while (true)
            {
                var msg = queueM.Pull();

                try
                {
                    container.GetCommand(msg.CommandName).Execute(msg.Arg);
                }
                catch (Exception exception)
                {
                        logger.Error("Main", exception.GetType().Name, exception.Message);
                }
            } 
        }
    }
}