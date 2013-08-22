// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordContainer.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//   Defines the Word Container type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace STrackerBackgroundWorker.TextValidators.Validators
{
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Lazy word container.
    /// </summary>
    public class WordContainer
    {
        /// <summary>
        /// Gets or sets the file directory.
        /// </summary>
        public string FileDirectory { get; set; }

        /// <summary>
        /// The words.
        /// </summary>
        private HashSet<string> words;

        /// <summary>
        /// Gets or sets the words.
        /// </summary>
        public HashSet<string> Words 
        {
            get
            {
                return this.words ?? (this.words = this.Initialize());
            }
        }

        /// <summary>
        /// The initialize the hash set.
        /// </summary>
        /// <returns>
        /// The <see cref="HashSet{T}"/>.
        /// </returns>
        private HashSet<string> Initialize()
        {
            var hashSet = new HashSet<string>();
            var xdoc = new XmlDocument();
            xdoc.Load(this.FileDirectory);
            var wordnodes = xdoc.SelectNodes("//Word");

            for (var i = 0; i < wordnodes.Count; i++)
            {
                var xmlNode = wordnodes.Item(i);
                hashSet.Add(xmlNode.InnerText.ToLower());
            }
            return hashSet;
        } 
    }
}
