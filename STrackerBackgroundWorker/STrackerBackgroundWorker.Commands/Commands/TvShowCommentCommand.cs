// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TvShowCommentCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Command for user comments for television shows.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.Commands.Commands
{
    using STrackerServer.DataAccessLayer.Core.TvShowsRepositories;
    using STrackerServer.DataAccessLayer.DomainEntities.AuxiliaryEntities;

    /// <summary>
    /// The television show comment command.
    /// </summary>
    public class TvShowCommentCommand : BaseCommentCommand
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly ITvShowCommentsRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvShowCommentCommand"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TvShowCommentCommand(ITvShowCommentsRepository repository)
            : base("tvShowCommentAdd")
        {
            this.repository = repository;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        public override void Execute(string arg)
        {
            // Get information.
            var splitArgs = arg.Split('|');
            var tvshowId = splitArgs[0];
            var userId = splitArgs[1];
            var commentText = splitArgs[2];

            if (this.ContainsOffensiveWords(commentText))
            {
                return;
            }

            // If the comment is valid insert into repository.
            var comment = new Comment { Body = commentText, UserId = userId };
            this.repository.AddComment(tvshowId, comment);
        }
    }
}