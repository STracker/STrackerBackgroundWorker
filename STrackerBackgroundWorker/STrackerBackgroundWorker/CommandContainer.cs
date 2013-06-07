// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandContainer.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Container of all commands.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater
{
    using System.Collections.Generic;

    using Ninject;

    using STrackerUpdate.Ninject;

    using STrackerUpdater.Commands.Commands;
    using STrackerUpdater.Commands.Core;

    /// <summary>
    /// The command container.
    /// </summary>
    public class CommandContainer
    {
        /// <summary>
        /// The commands.
        /// </summary>
        private readonly IDictionary<string, ICommand> commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainer"/> class.
        /// </summary>
        public CommandContainer()
        {
            this.commands = new Dictionary<string, ICommand>();

            using (IKernel kernel = new StandardKernel(new Module()))
            {
                var idCmd = kernel.Get<IdCommand>();
                var nameCmd = kernel.Get<NameCommand>();
                var tvshowCommentCmd = kernel.Get<TvShowCommentCommand>();
                var episodeCommentCmd = kernel.Get<EpisodeCommentCommand>();

                this.commands.Add(idCmd.CommandName, idCmd);
                this.commands.Add(nameCmd.CommandName, nameCmd);
                this.commands.Add(tvshowCommentCmd.CommandName, tvshowCommentCmd);
                this.commands.Add(episodeCommentCmd.CommandName, episodeCommentCmd);
            }
        }

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ICommand"/>.
        /// </returns>
        public ICommand GetCommand(string name)
        {
            return this.commands[name];
        }
    }
}