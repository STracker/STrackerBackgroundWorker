// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TheTvDbProvider.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Implementation of the TVDB provider.
//  More info at http://thetvdb.com/ and http://thetvdb.com/wiki/index.php?title=Programmers_API
//  The TVDB send the information only in XML format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.ExternalProviders.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Xml;

    using STrackerBackgroundWorker.ExternalProviders.Core;

    using STrackerServer.DataAccessLayer.DomainEntities;
    using STrackerServer.DataAccessLayer.DomainEntities.AuxiliaryEntities;

    /// <summary>
    /// The TVDB provider.
    /// </summary>
    public class TheTvDbProvider : ITvShowsInformationProvider
    {
        /// <summary>
        /// The not available.
        /// </summary>
        private const string NotAvailable = "N/A";

        /// <summary>
        /// The default actor photo.
        /// </summary>
        private const string DefaultActorPhoto = "https://dl.dropboxusercontent.com/u/2696848/default-profile-pic.jpg";

        /// <summary>
        /// The default poster.
        /// </summary>
        private const string DefaultPoster = "https://dl.dropboxusercontent.com/u/2696848/image-not-found.gif";

        /// <summary>
        /// The mirror path.
        /// </summary>
        private static readonly string MirrorPath = ConfigurationManager.AppSettings["TvDbMirrorPath"];

        /// <summary>
        /// The API key.
        /// </summary>
        private static readonly string ApiKey = ConfigurationManager.AppSettings["TvDbAPI"];

        /// <summary>
        /// The update type.
        /// </summary>
        private static readonly string UpdateType = ConfigurationManager.AppSettings["UpdateType"];

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IImageRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTvDbProvider"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TheTvDbProvider(IImageRepository repository)
        {
            this.repository = repository;
        }

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
        public void GetInformationByImdbId(string imdbId, out TvShow tvshow, out List<Season> seasons, out List<Episode> episodes)
        {
            var id = this.GetTheTvDbIdByImdbId(imdbId);
            this.GetInformation(id, out tvshow, out seasons, out episodes);
        }

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
        public void GetInformationByName(string name, out TvShow tvshow, out List<Season> seasons, out List<Episode> episodes)
        {
            var id = this.GetTheTvDbIdByName(name);
            this.GetInformation(id, out tvshow, out seasons, out episodes);
        }

        /// <summary>
        /// The get new episodes.
        /// </summary>
        /// <param name="episodes">
        /// The episodes.
        /// </param>
        public void GetNewEpisodes(out List<Episode> episodes)
        {
            var url = string.Format("{0}/api/{1}/updates/{2}.xml", MirrorPath, ApiKey, UpdateType);
            var xdoc = new XmlDocument();
            try
            {
                xdoc.Load(new XmlTextReader(url));
            }
            catch (Exception)
            {
                episodes = null;
                return;
            }

            var episodesNodes = xdoc.SelectNodes("//Episode");
            if (episodesNodes == null)
            {
                episodes = null;
                return;
            }

            episodes = new List<Episode>();
            for (var i = 0; i < episodesNodes.Count; i++)
            {
                var xmlNode = episodesNodes.Item(i);
                if (xmlNode == null)
                {
                    continue;
                }

                // Get IMDB id
                var idSerie = xmlNode.SelectSingleNode("Series");
                if (idSerie == null || idSerie.LastChild == null)
                {
                    continue;
                }

                var imdbid = this.GetImdbIdByTheTvDbId(idSerie.LastChild.Value);
                if (imdbid == null)
                {
                    continue;
                }

                var idEpi = xmlNode.SelectSingleNode("id");
                if (idEpi == null || idEpi.LastChild == null)
                {
                    continue;
                }
                
                var urlEpi = string.Format("{0}/api/{1}/episodes/{2}", MirrorPath, ApiKey, idEpi.LastChild.Value);
                var xdocEpi = new XmlDocument();
                try
                {
                    xdocEpi.Load(new XmlTextReader(urlEpi));
                }
                catch (Exception)
                {
                    continue;
                }

                var episode = this.GetEpisodesInformation(imdbid, xdocEpi).FirstOrDefault();
                episodes.Add(episode);
            }
        }

        /// <summary>
        /// The get information.
        /// </summary>
        /// <param name="id">
        /// The id.
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
        private void GetInformation(string id, out TvShow tvshow, out List<Season> seasons, out List<Episode> episodes)
        {
            if (id == null)
            {
                tvshow = null;
                seasons = null;
                episodes = null;
                return;
            }

            var url = string.Format("{0}/api/{1}/series/{2}/all", MirrorPath, ApiKey, id);
            var xdoc = new XmlDocument();

            try
            {
                xdoc.Load(new XmlTextReader(url));
            }
            catch (Exception)
            {
                tvshow = null;
                seasons = null;
                episodes = null;
                return;
            }

            // Get basic information and actors.
            tvshow = this.GetTvShowInformation(xdoc);
            if (tvshow == null)
            {
                seasons = null;
                episodes = null;
                return;
            }

            tvshow.Actors = this.GetActors(id);

            // Get seasons.
            seasons = this.GetSeasonsInformation(tvshow.Id, xdoc);

            // Get episodes.
            episodes = this.GetEpisodesInformation(tvshow.Id, xdoc);
        }

        /// <summary>
        /// The verify if exists by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTheTvDbIdByName(string name)
        {
            var url = string.Format("{0}/api/GetSeries.php?seriesname={1}", MirrorPath, name);
            var xdoc = new XmlDocument();
            var nameNormalized = name.ToLower();

            try
            {
                xdoc.Load(new XmlTextReader(url));
            }
            catch (Exception)
            {
                return null;
            }
            
            var seriesNode = xdoc.SelectNodes("//Series");

            // In this phase, STracker only accept requests for absolute name of the television show.
            if (seriesNode == null)
            {
                return null;
            }

            for (var i = 0; i < seriesNode.Count; i++)
            {
                var xmlNode = seriesNode.Item(i);
                if (xmlNode == null)
                {
                    continue;
                }

                var nodeName = xdoc.SelectSingleNode("//SeriesName");
                if (nodeName == null || nodeName.LastChild == null || !nodeName.LastChild.Value.ToLower().Equals(nameNormalized))
                {
                    continue;
                }

                var idNode = xdoc.SelectSingleNode("//seriesid");
                return (idNode != null && idNode.LastChild != null) ? idNode.LastChild.Value : null;
            }

            return null;
        }

        /// <summary>
        /// Auxiliary method for getting the TVDB id.
        /// </summary>
        /// <param name="imdbId">
        /// The IMDB id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTheTvDbIdByImdbId(string imdbId)
        {
            var url = string.Format("{0}/api/GetSeriesByRemoteID.php?imdbid={1}", MirrorPath, imdbId);
            var xdocId = new XmlDocument();
            try
            {
                xdocId.Load(new XmlTextReader(url));
            }
            catch (Exception)
            {
                return null;
            }
            
            var seriesIdNode = xdocId.SelectSingleNode("//seriesid");
            return (seriesIdNode != null && seriesIdNode.LastChild != null) ? seriesIdNode.LastChild.Value : null;
        }

        /// <summary>
        /// The get IMDB id by the TVDB id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetImdbIdByTheTvDbId(string id)
        {
            var url = string.Format("{0}/api/{1}/series/{2}", MirrorPath, ApiKey, id);
            var xdoc = new XmlDocument();
            try
            {
                xdoc.Load(new XmlTextReader(url));
            }
            catch (Exception)
            {
                return null;
            }

            var imdbIdNode = xdoc.SelectSingleNode("//IMDB_ID");
            return (imdbIdNode != null && imdbIdNode.LastChild != null) ? imdbIdNode.LastChild.Value : null;
        }

        /// <summary>
        /// Get a television show object with information filled.
        /// </summary>
        /// <param name="xdoc">
        /// The XML document.
        /// </param>
        /// <returns>
        /// The <see cref="TvShow"/>.
        /// </returns>
        private TvShow GetTvShowInformation(XmlNode xdoc)
        {
            var imdbIdNode = xdoc.SelectSingleNode("//IMDB_ID");
            if (imdbIdNode == null || imdbIdNode.LastChild == null)
            {
                return null;
            }

            var tvshow = new TvShow(imdbIdNode.LastChild.Value);

            var nameNode = xdoc.SelectSingleNode("//SeriesName");
            tvshow.Name = (nameNode != null && nameNode.LastChild != null) ? nameNode.LastChild.Value : NotAvailable;

            var descrNode = xdoc.SelectSingleNode("//Overview");
            tvshow.Description = (descrNode != null && descrNode.LastChild != null) ? descrNode.LastChild.Value : NotAvailable;

            var firstAiredNode = xdoc.SelectSingleNode("//FirstAired");
            tvshow.FirstAired = (firstAiredNode != null && firstAiredNode.LastChild != null) ? firstAiredNode.LastChild.Value : NotAvailable;

            var airDayNode = xdoc.SelectSingleNode("//Airs_DayOfWeek");
            tvshow.AirDay = (airDayNode != null && airDayNode.LastChild != null) ? airDayNode.LastChild.Value : NotAvailable;

            var airTimeNode = xdoc.SelectSingleNode("//Airs_Time");
            tvshow.AirTime = (airTimeNode != null && airTimeNode.LastChild != null) ? airTimeNode.LastChild.Value : NotAvailable;

            var runtimeNode = xdoc.SelectSingleNode("//Runtime");
            tvshow.Runtime = (runtimeNode != null && runtimeNode.LastChild != null) ? int.Parse(runtimeNode.LastChild.Value) : 0;

            var genreNode = xdoc.SelectSingleNode("//Genre");
            if (genreNode != null && genreNode.LastChild != null)
            {
                var genres = genreNode.LastChild.Value;
                var genresSplit = genres.Split('|');
                foreach (var genre in genresSplit.Where(genre => !genre.Trim().Equals(string.Empty)))
                {
                    var newGenre = new Genre(genre);
                    tvshow.Genres.Add(newGenre.GetSynopsis());
                }
            }

            var posterImageNode = xdoc.SelectSingleNode("//poster");
            if (posterImageNode != null && posterImageNode.LastChild != null)
            {
                tvshow.Poster = this.repository.Put(string.Format("{0}/banners/{1}", MirrorPath, posterImageNode.LastChild.Value));
            }
            else
            {
                tvshow.Poster = DefaultPoster;
            }

            return tvshow;
        }

        /// <summary>
        /// Auxiliary method for get the actors.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///       <cref>List</cref>
        ///     </see> .
        /// </returns>
        private List<Actor> GetActors(string id)
        {
            var xdoc = new XmlDocument();
            var url = string.Format("{0}/api/{1}/series/{2}/actors.xml", MirrorPath, ApiKey, id);
            
            try
            {
                xdoc.Load(new XmlTextReader(url));
            }
            catch (Exception)
            {
                return null;
            }

            var actorsNodes = xdoc.SelectNodes("//Actor");
            if (actorsNodes == null)
            {
                return null;
            }

            var actors = new List<Actor>();

            for (var i = 0; i < actorsNodes.Count; i++)
            {
                var xmlNode = actorsNodes.Item(i);
                if (xmlNode == null)
                {
                    continue;
                }

                // In this fase of the project, the actor dont need the key.
                var actor = new Actor();

                var nodeName = xmlNode.SelectSingleNode("Name");
                actor.Name = (nodeName != null && nodeName.LastChild != null) ? nodeName.LastChild.Value : NotAvailable;

                var characterNameNode = xmlNode.SelectSingleNode("Role");
                actor.CharacterName = (characterNameNode != null && characterNameNode.LastChild != null) ? characterNameNode.LastChild.Value : NotAvailable;

                var imageNode = xmlNode.SelectSingleNode("Image");
                if (imageNode != null && imageNode.LastChild != null)
                {
                    actor.Photo = this.repository.Put(string.Format("{0}/banners/{1}", MirrorPath, imageNode.LastChild.Value));
                }
                else
                {
                    actor.Photo = DefaultActorPhoto;
                }
                
                actors.Add(actor);
            }

            return actors;
        }

        /// <summary>
        /// Get seasons information.
        /// </summary>
        /// <param name="imdbId">
        /// The IMDB Id.
        /// </param>
        /// <param name="xdoc">
        /// The XML document.
        /// </param>
        /// <returns>
        /// The <see>
        ///       <cref>List</cref>
        ///     </see> .
        /// </returns>
        private List<Season> GetSeasonsInformation(string imdbId, XmlNode xdoc)
        {
            var seasonsNodes = xdoc.SelectNodes("//SeasonNumber");
            if (seasonsNodes == null)
            {
                return null;
            }

            var numbers = new HashSet<int>();
            for (var i = 0; i < seasonsNodes.Count; i++)
            {
                var xmlNode = seasonsNodes.Item(i);
                if (xmlNode == null)
                {
                    continue;
                }

                numbers.Add(int.Parse(xmlNode.LastChild.Value));
            }

            var list = new List<Season>();
            var enumerator = numbers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(new Season(new Season.SeasonId { TvShowId = imdbId, SeasonNumber = enumerator.Current }));
            }

            return list;
        }

        /// <summary>
        /// The get episodes information.
        /// </summary>
        /// <param name="imdbId">
        /// The IMDB id.
        /// </param>
        /// <param name="xdoc">
        /// The XML document.
        /// </param>
        /// <returns>
        /// The <see>
        ///       <cref>List</cref>
        ///     </see> .
        /// </returns>
        private List<Episode> GetEpisodesInformation(string imdbId, XmlNode xdoc)
        {
            var episodesNodes = xdoc.SelectNodes("//Episode");
            if (episodesNodes == null)
            {
                return null;
            }

            var list = new List<Episode>();
            for (var i = 0; i < episodesNodes.Count; i++)
            {
                var xmlNode = episodesNodes.Item(i);
                if (xmlNode == null)
                {
                    continue;
                }

                var episodeNumberNode = xmlNode.SelectSingleNode("EpisodeNumber");
                var seasonNumberNode = xmlNode.SelectSingleNode("SeasonNumber");
                if ((episodeNumberNode == null || episodeNumberNode.LastChild == null) || (seasonNumberNode == null || seasonNumberNode.LastChild == null))
                {
                    continue;
                }

                var episode = new Episode(new Episode.EpisodeId { TvShowId = imdbId, SeasonNumber = int.Parse(seasonNumberNode.LastChild.Value), EpisodeNumber = int.Parse(episodeNumberNode.LastChild.Value) });

                var nameNode = xmlNode.SelectSingleNode("EpisodeName");
                episode.Name = (nameNode != null && nameNode.LastChild != null) ? nameNode.LastChild.Value : NotAvailable;

                var descriptionNode = xmlNode.SelectSingleNode("Overview");
                episode.Description = (descriptionNode != null && descriptionNode.LastChild != null) ? descriptionNode.LastChild.Value : NotAvailable;

                var dateNote = xmlNode.SelectSingleNode("FirstAired");
                episode.Date = (dateNote != null && dateNote.LastChild != null) ? dateNote.LastChild.Value : NotAvailable;

                var directorNode = xmlNode.SelectSingleNode("Director");
                episode.Directors = new List<Person>();

                var directors = (directorNode != null && directorNode.LastChild != null) ? directorNode.LastChild.Value.Split('|') : new string[0];
                foreach (var director in directors.Where(director => director != string.Empty))
                {
                    episode.Directors.Add(new Person
                        {
                            Name = director.Trim(),
                            Photo = DefaultActorPhoto
                        });
                }

                var guestsNode = xmlNode.SelectSingleNode("GuestStars");
                episode.GuestActors = new List<Actor>();

                var guests = (guestsNode != null && guestsNode.LastChild != null) ? guestsNode.LastChild.Value.Split('|') : new string[0];
                foreach (var guest in guests.Where(guest => guest != string.Empty))
                {
                    episode.GuestActors.Add(new Actor
                        {
                            Name = guest.Trim(), 
                            CharacterName = NotAvailable, 
                            Photo = DefaultActorPhoto
                        });
                }

                list.Add(episode);

                var filenameNode = xmlNode.SelectSingleNode("filename");
                var filename = (filenameNode != null && filenameNode.LastChild != null) ? filenameNode.LastChild.Value : null;

                episode.Poster = filename != null ? this.repository.Put(string.Format("{0}/banners/{1}", MirrorPath, filename)) : DefaultPoster;
            }

            return list;
        }
    }
}