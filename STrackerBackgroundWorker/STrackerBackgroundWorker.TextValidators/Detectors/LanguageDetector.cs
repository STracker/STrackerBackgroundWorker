// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageDetector.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Language detector that uses the api of http://detectlanguage.com/
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Detectors
{
    using System.IO;
    using System.Linq;
    using System.Net;

    using Newtonsoft.Json;

    using STrackerBackgroundWorker.TextValidators.Core;
    using STrackerBackgroundWorker.TextValidators.Detectors.LDModels;

    /// <summary>
    /// The language detector.
    /// </summary>
    public class LanguageDetector : ILanguageDetector
    {
        /// <summary>
        /// The request uri format.
        /// </summary>
        private const string RequestUriFormat = "http://ws.detectlanguage.com/0.2/detect?key={1}&q={0}";

        /// <summary>
        /// The API key.
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDetector"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The API key.
        /// </param>
        public LanguageDetector(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Detects the language of the text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Detect(string text)
        {
            var request = WebRequest.CreateHttp(this.GetRequestUri(text));
            var response = request.GetResponse();
            var stream = response.GetResponseStream();

            if (stream == null)
            {
                return null;
            }

            using (var streamReader = new StreamReader(stream))
            {
                var jsonReader = new JsonTextReader(streamReader);
                var serializer = new JsonSerializer();
                var responseObj = serializer.Deserialize<LdResponse>(jsonReader);

                if (responseObj.Data.Detections == null || responseObj.Data.Detections.Length == 0)
                {
                    return null;
                }

                return responseObj.Data.Detections
                    .OrderByDescending(detection => detection.Confidence)
                    .First()
                    .Language;
            }
        }

        /// <summary>
        /// Get the API request uri.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRequestUri(string text)
        {
            return string.Format(RequestUriFormat, text, this.apiKey);
        }
    }
}
