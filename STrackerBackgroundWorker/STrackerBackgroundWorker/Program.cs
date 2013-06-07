// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Entry point for STrackerUpdater.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater
{
    using Ninject;

    using STrackerUpdate.Ninject;

    using STrackerUpdater.RabbitMQ;

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
            using (IKernel kernel = new StandardKernel(new Module()))
            {
                queueM = kernel.Get<QueueManager>();
            }

            var container = new CommandContainer();

            while (true)
            {
                var msg = queueM.Pull();
                container.GetCommand(msg.CommandName).Execute(msg.Arg);
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns
    }
}