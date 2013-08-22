// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILanguageDetector.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Used to detect the language of a certain text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Core
{
    /// <summary>
    /// The Language Detector interface.
    /// </summary>
    public interface ILanguageDetector
    {
        /// <summary>
        /// Detects the language of the text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Detect(string text);
    }
}
