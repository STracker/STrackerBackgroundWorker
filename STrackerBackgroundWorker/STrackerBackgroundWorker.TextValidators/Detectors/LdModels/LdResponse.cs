// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LdResponse.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Defines the Language detector response type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Detectors.LDModels
{
    using Newtonsoft.Json;

    using STrackerBackgroundWorker.TextValidators.Detectors.LdModels;

    /// <summary>
    /// The language detector JSON response.
    /// </summary>
    public class LdResponse
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public LdData Data { get; set; }
    }
}
