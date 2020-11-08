using System;

namespace Agfinn.PodcastIndex.Lib
{
    public class FeedResponse : PodcastIndexResponseBase
    {
        public PodcastFeed feed { get; set; }
    }
}
