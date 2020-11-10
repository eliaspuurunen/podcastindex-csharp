using System;

namespace Agfinn.PodcastIndex.Lib
{
    public class AddFeedResponse : PodcastIndexResponseBase
    {
        public bool existed { get; set; }
        public int feedId { get; set; }
    }
}
