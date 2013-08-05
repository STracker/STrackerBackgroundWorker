// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageRepository.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Defines the IImageRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.ExternalProviders.Core
{
    /// <summary>
    /// The ImageRepository interface.
    /// </summary>
    public interface IImageRepository
    {
        /// <summary>
        /// The put.
        /// </summary>
        /// <param name="imageUrl">
        /// The image url.
        /// </param>
        /// <returns>
        /// The resulting url.
        /// </returns>
        string Put(string imageUrl);
    }
}
