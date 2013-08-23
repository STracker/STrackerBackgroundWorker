// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LdDetection.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Defines the LdDetection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Detectors.LdModels
{
    /// <summary>
    /// The language detector JSON response data detection.
    /// </summary>
    public class LdDetection
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is reliable.
        /// </summary>
        public bool IsReliable { get; set; }

        /// <summary>
        /// Gets or sets the confidence.
        /// </summary>
        public double Confidence { get; set; }
    }
}
