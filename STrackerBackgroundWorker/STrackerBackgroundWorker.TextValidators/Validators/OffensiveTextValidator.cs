// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OffensiveTextValidator.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Defines the Offensive Text Validator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Validators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using STrackerBackgroundWorker.TextValidators.Core;

    /// <summary>
    /// The offensive text validator.
    /// </summary>
    public class OffensiveTextValidator : ITextValidator
    {
        /// <summary>
        /// The words directory.
        /// </summary>
        public const string WordsDirectory = "../../Words";

        /// <summary>
        /// The offensive words.
        /// </summary>
        private readonly IDictionary<string, WordContainer> offensiveWords;

        /// <summary>
        /// The language detector.
        /// </summary>
        private readonly ILanguageDetector languageDetector;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffensiveTextValidator"/> class.
        /// </summary>
        /// <param name="languageDetector">
        /// The language detector.
        /// </param>
        public OffensiveTextValidator(ILanguageDetector languageDetector)
        {
            this.languageDetector = languageDetector;
            this.offensiveWords = new Dictionary<string, WordContainer>();

            foreach (var fileInfo in Directory.GetFiles(WordsDirectory).Select(file => new FileInfo(file)))
            {
                var language = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                this.offensiveWords.Add(language, new WordContainer { FileDirectory = fileInfo.FullName });
            }
        }

        /// <summary>
        /// Validates the text.
        /// </summary>
        /// <param name="text">
        /// The text that will be validated.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Validate(string text)
        {
            var language = this.languageDetector.Detect(text);

            if (language == null || !this.offensiveWords.ContainsKey(language))
            {
                return true;
            }

            var container = this.offensiveWords[language];

            foreach (var textWord in text.Split(' '))
            {
                if (container.Words.Contains(textWord.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
