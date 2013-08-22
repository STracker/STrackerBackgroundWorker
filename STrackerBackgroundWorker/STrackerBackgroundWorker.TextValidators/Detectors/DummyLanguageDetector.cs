// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyLanguageDetector.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Defines the DummyLanguageDetector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Detectors
{
    using STrackerBackgroundWorker.TextValidators.Core;

    /// <summary>
    /// The dummy language validator.
    /// </summary>
    public class DummyLanguageDetector : ILanguageDetector
    {
        /// <summary>
        /// The detect.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Detect(string text)
        {
            return "en";
        }
    }
}
