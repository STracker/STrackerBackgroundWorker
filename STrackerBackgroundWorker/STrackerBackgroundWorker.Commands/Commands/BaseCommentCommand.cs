// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCommentCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Base command for user comments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.Commands.Commands
{
    using STrackerBackgroundWorker.Commands.Core;

    /// <summary>
    /// The base comment command.
    /// </summary>
    public abstract class BaseCommentCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommentCommand"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        protected BaseCommentCommand(string name)
        {
            this.CommandName = name;
        }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        public abstract void Execute(string arg);

        /// <summary>
        /// Verify if the comment contains offensive words.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ContainsOffensiveWords(string comment)
        {
            // TODO
            return false;
        }
    }
}