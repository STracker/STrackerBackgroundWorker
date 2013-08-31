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
    using STrackerBackgroundWorker.TextValidators.Validators;

    using STrackerServer.DataAccessLayer.Core.EpisodesRepositories;
    using STrackerServer.DataAccessLayer.Core.UsersRepositories;
    using STrackerServer.DataAccessLayer.DomainEntities;
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
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeCommentCommand"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="usersRepository">
        /// The users repository.
        /// </param>
        /// <param name="offensiveTextValidator">
        /// The offensive Text Validator.
        /// </param>
        public EpisodeCommentCommand(IEpisodeCommentsRepository repository, IUsersRepository usersRepository, OffensiveTextValidator offensiveTextValidator)
            : base("episodeCommentAdd", offensiveTextValidator)
        {
            this.repository = repository;
            this.usersRepository = usersRepository;
        }

        /// <summary>
        /// The comment command execute.
        /// </summary>
        /// <param name="arg">
        /// The comment.
        /// </param>
        protected override void CommentCommandExecute(string arg)
        {
            // Get information.
            var splitArgs = arg.Split('|');
            var tvshowId = splitArgs[0];
            var seasonNumber = splitArgs[1];
            var episodeNumber = splitArgs[2];
            var userId = splitArgs[3];
            var commentText = splitArgs[4];

            // If the comment is valid insert into repository.
            var comment = new Comment { Body = commentText, User = this.usersRepository.Read(userId).GetSynopsis() };

            comment.Uri = string.Format("tvshows/{0}/seasons/{1}/episodes/{2}/comments/{3}", tvshowId, seasonNumber, episodeNumber, comment.Id);

            this.repository.AddComment(new Episode.EpisodeId { TvShowId = tvshowId, SeasonNumber = int.Parse(seasonNumber), EpisodeNumber = int.Parse(episodeNumber) }, comment);
        }
    }
}