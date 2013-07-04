// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EpisodeCommentCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Command for add user comments for episodes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.Commands.Commands
{
    using System;

    using STrackerServer.DataAccessLayer.Core.EpisodesRepositories;
    using STrackerServer.DataAccessLayer.DomainEntities.AuxiliaryEntities;

    /// <summary>
    /// The episode comment command.
    /// </summary>
    public class EpisodeCommentCommand : BaseCommentCommand
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IEpisodeCommentsRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeCommentCommand"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public EpisodeCommentCommand(IEpisodeCommentsRepository repository)
            : base("episodeCommentAdd")
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
            var seasonNumber = splitArgs[1];
            var episodeNumber = splitArgs[2];
            var userId = splitArgs[3];
            var commentText = splitArgs[4];

            if (this.ContainsOffensiveWords(commentText))
            {
                return;
            }

            // If the comment is valid insert into repository.
            var comment = new Comment { Body = commentText, UserId = userId };
            this.repository.AddComment(new Tuple<string, int, int>(tvshowId, int.Parse(seasonNumber), int.Parse(episodeNumber)), comment);
        }
    }
}