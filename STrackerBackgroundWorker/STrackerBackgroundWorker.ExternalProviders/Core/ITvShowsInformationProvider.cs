// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITvShowsInformationProvider.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Interface for information provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater.ExternalProviders.Core
{
    using System.Collections.Generic;

    using STrackerServer.DataAccessLayer.DomainEntities;

    /// <summary>
    /// The Information provider interface.
    /// </summary>
    public interface ITvShowsInformationProvider
    {
        /// <summary>
        /// The get information by IMDB id.
        /// </summary>
        /// <param name="imdbId">
        /// The IMDB Id.
        /// </param>
        /// <param name="tvshow">
        /// The television show.
        /// </param>
        /// <param name="seasons">
        /// The seasons.
        /// </param>
        /// <param name="episodes">
        /// The episodes.
        /// </param>
        void GetInformationByImdbId(string imdbId, out TvShow tvshow, out List<Season> seasons, out List<Episode> episodes);

        /// <summary>
        /// The get information by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="tvshow">
        /// The television show.
        /// </param>
        /// <param name="seasons">
        /// The seasons.
        /// </param>
        /// <param name="episodes">
        /// The episodes.
        /// </param>
        void GetInformationByName(string name, out TvShow tvshow, out List<Season> seasons, out List<Episode> episodes);
    }
}