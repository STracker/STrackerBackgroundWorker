// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Module.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Ninject module for STrackerUpdater dependencies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.Ninject
{
    using System.Configuration;

    using MongoDB.Driver;

    using global::Ninject.Modules;

    using global::RabbitMQ.Client;

    using STrackerBackgroundWorker.Commands.Commands;
    using STrackerBackgroundWorker.ExternalProviders;
    using STrackerBackgroundWorker.ExternalProviders.Core;
    using STrackerBackgroundWorker.ExternalProviders.Providers;
    using STrackerBackgroundWorker.RabbitMQ;

    using STrackerServer.DataAccessLayer.Core.EpisodesRepositories;
    using STrackerServer.DataAccessLayer.Core.SeasonsRepositories;
    using STrackerServer.DataAccessLayer.Core.TvShowsRepositories;
    using STrackerServer.DataAccessLayer.Core.UsersRepositories;
    using STrackerServer.Repository.MongoDB.Core.EpisodesRepositories;
    using STrackerServer.Repository.MongoDB.Core.SeasonsRepositories;
    using STrackerServer.Repository.MongoDB.Core.TvShowsRepositories;
    using STrackerServer.Repository.MongoDB.Core.UsersRepositories;

    /// <summary>
    /// The module for STRACKER.
    /// </summary>
    public class Module : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            // MongoDB stuff dependencies...
            this.Bind<MongoUrl>().ToSelf().InSingletonScope().WithConstructorArgument("url", ConfigurationManager.AppSettings["MongoDBURL"]);

            // MongoClient class is thread safe.
            this.Bind<MongoClient>().ToSelf().InSingletonScope();

            // Television shows stuff dependencies...
            this.Bind<ITvShowsRepository>().To<TvShowsRepository>();
            this.Bind<IGenresRepository>().To<GenresRepository>();
            this.Bind<ITvShowCommentsRepository>().To<TvShowCommentsRepository>();
            this.Bind<ITvShowRatingsRepository>().To<TvShowRatingsRepository>();

            // Seasons stuff dependencies...
            this.Bind<ISeasonsRepository>().To<SeasonsRepository>();

            // Episodes stuff dependencies...
            this.Bind<IEpisodesRepository>().To<EpisodesRepository>();
            this.Bind<IEpisodeCommentsRepository>().To<EpisodeCommentsRepository>();
            this.Bind<IEpisodeRatingsRepository>().To<EpisodeRatingsRepository>();

            // Users stuff dependencies...
            this.Bind<IUsersRepository>().To<UsersRepository>();

            // Providers dependencies...
            this.Bind<ITvShowsInformationProvider>().To<TheTvDbProvider>();
            this.Bind<TvShowsInformationManager>().ToSelf().InSingletonScope();

            // Commands dependencies...
            this.Bind<IdCommand>().ToSelf().InSingletonScope();
            this.Bind<NameCommand>().ToSelf().InSingletonScope();
            this.Bind<TvShowCommentCommand>().ToSelf().InSingletonScope();
            this.Bind<EpisodeCommentCommand>().ToSelf().InSingletonScope();

            // Queue dependencies...
            this.Bind<ConnectionFactory>().ToSelf().InSingletonScope().WithPropertyValue("Uri", ConfigurationManager.AppSettings["RabbitMQUri"]);
            this.Bind<QueueManager>().ToSelf().InSingletonScope();
        }
    }
}