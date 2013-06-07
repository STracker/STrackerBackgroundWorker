// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseTvshowCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Command for television shows id.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater.Commands.Commands
{
    using System.Collections.Generic;

    using STrackerServer.DataAccessLayer.Core.EpisodesRepositories;
    using STrackerServer.DataAccessLayer.Core.SeasonsRepositories;
    using STrackerServer.DataAccessLayer.Core.TvShowsRepositories;
    using STrackerServer.DataAccessLayer.DomainEntities;

    using STrackerUpdater.Commands.Core;
    using STrackerUpdater.ExternalProviders;

    /// <summary>
    /// The base television show command.
    /// </summary>
    public abstract class BaseTvshowCommand : ICommand
    {
        /// <summary>
        /// The television shows repository.
        /// </summary>
        protected readonly ITvShowsRepository TvshowsRepository;

        /// <summary>
        /// The seasons repository.
        /// </summary>
        protected readonly ISeasonsRepository SeasonsRepository;

        /// <summary>
        /// The episodes repository.
        /// </summary>
        protected readonly IEpisodesRepository EpisodesRepository;

        /// <summary>
        /// The info manager.
        /// </summary>
        protected readonly TvShowsInformationManager InfoManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTvshowCommand"/> class.
        /// </summary>
        /// <param name="tvshowsRepository">
        /// The television shows repository.
        /// </param>
        /// <param name="seasonsRepository">
        /// The seasons repository.
        /// </param>
        /// <param name="episodesRepository">
        /// The episodes repository.
        /// </param>
        /// <param name="infoManager">
        /// The info Manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        protected BaseTvshowCommand(ITvShowsRepository tvshowsRepository, ISeasonsRepository seasonsRepository, IEpisodesRepository episodesRepository,  TvShowsInformationManager infoManager, string name)
        {
            this.TvshowsRepository = tvshowsRepository;
            this.SeasonsRepository = seasonsRepository;
            this.EpisodesRepository = episodesRepository;
            this.InfoManager = infoManager;
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
        /// The add to database.
        /// </summary>
        /// <param name="tvshow">
        /// The television show.
        /// </param>
        /// <param name="seasons">
        /// The seasons.
        /// </param>
        /// <param name="episodes">
        /// The episodes.
        /// </param>
        protected void AddToDatabase(TvShow tvshow, List<Season> seasons, List<Episode> episodes)
        {
            // Add to database.
            if (!this.TvshowsRepository.Create(tvshow))
            {
                return;
            }
            
            this.SeasonsRepository.CreateAll(seasons);
            this.EpisodesRepository.CreateAll(episodes);
        }
    }
}