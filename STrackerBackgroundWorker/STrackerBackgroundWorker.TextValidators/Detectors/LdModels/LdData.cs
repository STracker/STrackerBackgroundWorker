// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LdData.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   The language detector json response data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Detectors.LdModels
{
    using Newtonsoft.Json;

    /// <summary>
    /// The language detector JSON response data.
    /// </summary>
    public class LdData
    {
        /// <summary>
        /// Gets or sets the detections.
        /// </summary>
        public LdDetection[] Detections { get; set; }
    }
}
