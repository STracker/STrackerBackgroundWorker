// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITextValidator.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Performs text validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Core
{
    /// <summary>
    /// The Text Validator interface.
    /// </summary>
    public interface ITextValidator
    {
        /// <summary>
        /// Validates the text.
        /// </summary>
        /// <param name="text">
        /// The text that will be validated.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Validate(string text);
    }
}
