using System;
using System.Collections.Generic;
using System.Linq;

namespace Agfinn.PodcastIndex.Lib
{
    /// <summary>
    /// Represents search options for podcast feed retrieval.
    /// </summary>
    public class SearchOptions
    {
        /// <summary>
        /// The maximum number of episodes to retrieve. Defaults to 10.
        /// </summary>
        public int Maximum { get; set; } = 10;

        /// <summary>
        /// A list of RSS language codes to retrieve.
        /// </summary>
        public List<string> Language { get; set; } = new List<string>();

        /// <summary>
        /// A list of category IDs or category names to show in results.
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>();

        /// <summary>
        /// A list of category IDs or category names to exclude in results.
        /// </summary>
        public List<string> ExcludeCategories { get; set; } = new List<string>();
    }
}
