// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Command for television shows id.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.Commands.Commands
{
    using System.Collections.Generic;

    using STrackerBackgroundWorker.ExternalProviders;

    using STrackerServer.DataAccessLayer.Core.EpisodesRepositories;
    using STrackerServer.DataAccessLayer.Core.SeasonsRepositories;
    using STrackerServer.DataAccessLayer.Core.TvShowsRepositories;
    using STrackerServer.DataAccessLayer.DomainEntities;

    /// <summary>
    /// The id command.
    /// </summary>
    public class IdCommand : BaseTvshowCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdCommand"/> class.
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
        public IdCommand(ITvShowsRepository tvshowsRepository, ISeasonsRepository seasonsRepository, IEpisodesRepository episodesRepository, TvShowsInformationManager infoManager)
            : base(tvshowsRepository, seasonsRepository, episodesRepository, infoManager, "id")
        {
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        public override void Execute(string arg)
        {
            // Verify if exists or already exists in database.
            if (this.TvshowsRepository.Read(arg) != null)
            {
                return;
            }

            TvShow tvshow;
            List<Season> seasons;
            List<Episode> episodes;

            var provider = this.InfoManager.GetDefaultProvider();
            provider.GetInformationByImdbId(arg, out tvshow, out seasons, out episodes);
          
            if (tvshow != null)
            {
                this.AddToDatabase(tvshow, seasons, episodes);
                return;
            }

            var providers = this.InfoManager.GetFromProvidersFolder();
            foreach (var prov in providers)
            {
                prov.GetInformationByImdbId(arg, out tvshow, out seasons, out episodes);
                if (tvshow == null)
                {
                    continue;
                }

                this.AddToDatabase(tvshow, seasons, episodes);
                return;
            }
        }
    }
}