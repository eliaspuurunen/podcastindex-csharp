using System;
using System.Collections.Generic;

namespace Agfinn.PodcastIndex.Lib
{
    public class SearchResponse : PodcastIndexResponseBase
    {
        public List<PodcastFeed> feeds { get; set; }
    }
}
