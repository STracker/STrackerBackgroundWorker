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
        /// The mirror path.
        /// </summary>
        private static readonly string MirrorPath = ConfigurationManager.AppSettings["TvDbMirrorPath"];

        /// <summary>
        /// The API key.
        /// </summary>
        private static readonly string ApiKey = ConfigurationManager.AppSettings["TvDbAPI"];

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
            var id = GetTheTvDbIdByImdbId(imdbId);
            GetInformation(id, out tvshow, out seasons, out episodes);
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
            var id = GetTheTvDbIdByName(name);
            GetInformation(id, out tvshow, out seasons, out episodes);
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
        private static void GetInformation(string id, out TvShow tvshow, out List<Season> seasons, out List<Episode> episodes)
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
            tvshow = GetTvShowInformation(xdoc);
            if (tvshow == null)
            {
                seasons = null;
                episodes = null;
                return;
            }

            tvshow.Actors = GetActors(id);

            // Get seasons.
            seasons = GetSeasonsInformation(tvshow.TvShowId, xdoc);

            // Get episodes.
            episodes = GetEpisodesInformation(tvshow.TvShowId, xdoc);
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
        private static string GetTheTvDbIdByName(string name)
        {
            var url = string.Format("{0}/api/GetSeries.php?seriesname={1}", MirrorPath, name);
            var xdoc = new XmlDocument();
           
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
                if (nodeName == null || nodeName.LastChild == null || !nodeName.LastChild.Value.Equals(name))
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
        private static string GetTheTvDbIdByImdbId(string imdbId)
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

            return seriesIdNode != null ? seriesIdNode.LastChild.Value : null;
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
        private static TvShow GetTvShowInformation(XmlNode xdoc)
        {
            var imdbIdNode = xdoc.SelectSingleNode("//IMDB_ID");
            if (imdbIdNode == null || imdbIdNode.LastChild == null)
            {
                return null;
            }

            var tvshow = new TvShow(imdbIdNode.LastChild.Value);

            var nameNode = xdoc.SelectSingleNode("//SeriesName");
            tvshow.Name = (nameNode != null && nameNode.LastChild != null) ? nameNode.LastChild.Value : null;

            var descrNode = xdoc.SelectSingleNode("//Overview");
            tvshow.Description = (descrNode != null && descrNode.LastChild != null) ? descrNode.LastChild.Value : null;

            var firstAiredNode = xdoc.SelectSingleNode("//FirstAired");
            tvshow.FirstAired = (firstAiredNode != null && firstAiredNode.LastChild != null) ? firstAiredNode.LastChild.Value : null;

            var airDayNode = xdoc.SelectSingleNode("//Airs_DayOfWeek");
            tvshow.AirDay = (airDayNode != null && airDayNode.LastChild != null) ? airDayNode.LastChild.Value : null;

            var airTimeNode = xdoc.SelectSingleNode("//Airs_Time");
            tvshow.AirTime = (airTimeNode != null && airTimeNode.LastChild != null) ? airTimeNode.LastChild.Value : null;

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
                tvshow.Poster = new Artwork
                        {
                            ImageUrl = string.Format("{0}/banners/{1}", MirrorPath, posterImageNode.LastChild.Value)
                        };
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
        private static List<Actor> GetActors(string id)
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
                actor.Name = (nodeName != null && nodeName.LastChild != null) ? nodeName.LastChild.Value : null;

                var characterNameNode = xmlNode.SelectSingleNode("Role");
                actor.CharacterName = (characterNameNode != null && characterNameNode.LastChild != null) ? characterNameNode.LastChild.Value : null;

                var imageNode = xmlNode.SelectSingleNode("Image");
                if (imageNode != null && imageNode.LastChild != null)
                {
                    actor.Photo = new Artwork { ImageUrl = string.Format("{0}/banners/{1}", MirrorPath, imageNode.LastChild.Value) };
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
        private static List<Season> GetSeasonsInformation(string imdbId, XmlNode xdoc)
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
                list.Add(new Season(new Tuple<string, int>(imdbId, enumerator.Current)));
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
        private static List<Episode> GetEpisodesInformation(string imdbId, XmlNode xdoc)
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
                if (episodeNumberNode == null || seasonNumberNode == null)
                {
                    continue;
                }

                var episode = new Episode(new Tuple<string, int, int>(imdbId, int.Parse(seasonNumberNode.LastChild.Value), int.Parse(episodeNumberNode.LastChild.Value)));

                var nameNode = xmlNode.SelectSingleNode("EpisodeName");
                episode.Name = (nameNode != null && nameNode.LastChild != null) ? nameNode.LastChild.Value : null;

                var descriptionNode = xmlNode.SelectSingleNode("Overview");
                episode.Description = (descriptionNode != null && descriptionNode.LastChild != null) ? descriptionNode.LastChild.Value : null;

                var dateNote = xmlNode.SelectSingleNode("FirstAired");
                episode.Date = (dateNote != null && dateNote.LastChild != null) ? dateNote.LastChild.Value : null;

                var directorNode = xmlNode.SelectSingleNode("Director");
                episode.Directors = new List<Person>();

                var directors = (directorNode != null && directorNode.LastChild != null) ? directorNode.LastChild.Value.Split('|') : new string[0];
                foreach (var director in directors.Where(director => director != string.Empty))
                {
                    episode.Directors.Add(new Person
                        {
                            Name = director.Trim()
                        });
                }

                var guestsNode = xmlNode.SelectSingleNode("GuestStars");
                episode.GuestActors = new List<Actor>();

                var guests = (guestsNode != null && guestsNode.LastChild != null) ? guestsNode.LastChild.Value.Split('|') : new string[0];
                foreach (var guest in guests.Where(guest => guest != string.Empty))
                {
                    episode.GuestActors.Add(new Actor { Name = guest.Trim() });
                }

                list.Add(episode);

                var filenameNode = xmlNode.SelectSingleNode("filename");
                var filename = (filenameNode != null && filenameNode.LastChild != null) ? filenameNode.LastChild.Value : null;

                if (filename != null)
                {
                    episode.Poster = new Artwork { ImageUrl = string.Format("{0}/banners/{1}", MirrorPath, filename) };
                }
            }

            return list;
        }
    }
}