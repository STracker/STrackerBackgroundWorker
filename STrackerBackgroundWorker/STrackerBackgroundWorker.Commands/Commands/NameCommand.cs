// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameCommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Command for television shows name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater.Commands.Commands
{
    using System.Collections.Generic;

    using STrackerServer.DataAccessLayer.Core.EpisodesRepositories;
    using STrackerServer.DataAccessLayer.Core.SeasonsRepositories;
    using STrackerServer.DataAccessLayer.Core.TvShowsRepositories;
    using STrackerServer.DataAccessLayer.DomainEntities;

    using STrackerUpdater.ExternalProviders;

    /// <summary>
    /// The name command.
    /// </summary>
    public class NameCommand : BaseTvshowCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameCommand"/> class.
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
        public NameCommand(ITvShowsRepository tvshowsRepository, ISeasonsRepository seasonsRepository, IEpisodesRepository episodesRepository, TvShowsInformationManager infoManager)
            : base(tvshowsRepository, seasonsRepository, episodesRepository, infoManager, "name")
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
            provider.GetInformationByName(arg, out tvshow, out seasons, out episodes);

            if (tvshow != null)
            {
                this.AddToDatabase(tvshow, seasons, episodes);
                return;
            }

            var providers = this.InfoManager.GetFromProvidersFolder();
            foreach (var prov in providers)
            {
                prov.GetInformationByName(arg, out tvshow, out seasons, out episodes);
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