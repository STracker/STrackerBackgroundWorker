﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudinaryRepository.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   The cloudinary repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.ExternalProviders.Repositories
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;

    using STrackerBackgroundWorker.ExternalProviders.Core;

    /// <summary>
    /// The cloudinary repository.
    /// </summary>
    public class CloudinaryRepository : IImageRepository
    {
        /// <summary>
        /// The provider.
        /// </summary>
        private readonly Cloudinary provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudinaryRepository"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public CloudinaryRepository(Cloudinary provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// The put.
        /// </summary>
        /// <param name="imageUrl">
        /// The image url.
        /// </param>
        /// <returns>
        /// The resulting url.
        /// </returns>
        public string Put(string imageUrl)
        {
            var result = this.provider.Upload(new ImageUploadParams { File = new FileDescription(imageUrl) });
            return result.Uri.AbsoluteUri;
        }
    }
}
