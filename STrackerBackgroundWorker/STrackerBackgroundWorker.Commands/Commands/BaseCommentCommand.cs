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
    using System.Linq;

    using STrackerBackgroundWorker.Commands.Core;
    using STrackerBackgroundWorker.TextValidators.Core;

    /// <summary>
    /// The base comment command.
    /// </summary>
    public abstract class BaseCommentCommand : ICommand
    {
        /// <summary>
        /// The text validators.
        /// </summary>
        private readonly ITextValidator[] textValidators;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommentCommand"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="textValidators">
        /// The text validators.
        /// </param>
        protected BaseCommentCommand(string name, params ITextValidator[] textValidators)
        {
            this.CommandName = name;
            this.textValidators = textValidators;
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
        /// The comment execute.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ValidateComment(string comment)
        {
            if (this.textValidators == null)
            {
                return true;
            }

            return !this.textValidators.Any(textValidator => !textValidator.Validate(comment));
        }
    }
}