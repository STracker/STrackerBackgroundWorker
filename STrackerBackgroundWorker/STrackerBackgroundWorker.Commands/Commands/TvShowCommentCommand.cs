﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TvShowCommentCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Command for user comments for television shows.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.Commands.Commands
{
    using STrackerBackgroundWorker.TextValidators.Core;

    using STrackerServer.DataAccessLayer.Core.TvShowsRepositories;
    using STrackerServer.DataAccessLayer.Core.UsersRepositories;
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
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvShowCommentCommand"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="usersRepository">
        /// The users repository.
        /// </param>
        /// <param name="textValidator">
        /// The text validator.
        /// </param>
        public TvShowCommentCommand(ITvShowCommentsRepository repository, IUsersRepository usersRepository, ITextValidator textValidator)
            : base("tvShowCommentAdd", textValidator)
        {
            this.repository = repository;
            this.usersRepository = usersRepository;
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
            var comment = new Comment
                {
                    Body = commentText, 
                    User = this.usersRepository.Read(userId).GetSynopsis()
                };

            comment.Uri = string.Format("tvshows/{0}/comments/{1}", tvshowId, comment.Id);

            this.repository.AddComment(tvshowId, comment);
        }
    }
}