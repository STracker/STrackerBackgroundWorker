// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TvShowsInformationManager.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Implementation of ITvShowsInformationManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerUpdater.ExternalProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using STrackerUpdater.ExternalProviders.Core;

    /// <summary>
    /// The television shows information manager.
    /// </summary>
    public class TvShowsInformationManager
    {
        /// <summary>
        /// The default provider.
        /// </summary>
        private readonly ITvShowsInformationProvider defaultProvider;

        /// <summary>
        /// The providers.
        /// </summary>
        private readonly List<ITvShowsInformationProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvShowsInformationManager"/> class.
        /// </summary>
        /// <param name="defaultProvider">
        /// The default Provider.
        /// </param>
        public TvShowsInformationManager(ITvShowsInformationProvider defaultProvider)
        {
            this.defaultProvider = defaultProvider;
            this.providers = new List<ITvShowsInformationProvider>();
        }

        /// <summary>
        /// The get default provider.
        /// </summary>
        /// <returns>
        /// The <see cref="ITvShowsInformationProvider"/>.
        /// </returns>
        /// TheTvDb provider its the default.
        public ITvShowsInformationProvider GetDefaultProvider()
        {
            return this.defaultProvider;
        }

        /// <summary>
        /// The get from providers folder.
        /// </summary>
        /// <returns>
        /// The <see>
        ///       <cref>List</cref>
        ///     </see> .
        /// </returns>
        public List<ITvShowsInformationProvider> GetFromProvidersFolder()
        {
            // Load assembly files
           var files = Directory.GetFiles("../../../STrackerUpdater.ExternalProviders/Providers");

            if (files.Length <= this.providers.Count)
            {
                return this.providers;
            }

            foreach (var assembly in from file in files select new FileInfo(file) into fileInfo where fileInfo.Extension.Equals(".dll") select Assembly.LoadFile(fileInfo.FullName))
            {
                this.providers.AddRange(from type in assembly.ExportedTypes where typeof(ITvShowsInformationProvider).IsAssignableFrom(type) select (ITvShowsInformationProvider)Activator.CreateInstance(type));
            }

            return this.providers;
        }
    }
}