using System;
using System.Collections.Generic;

namespace Agfinn.PodcastIndex.Lib
{
    public class PodcastEpisodeListResponse : PodcastIndexResponseBase
    {
        public List<Episode> items { get; set; }
    }

    public class RandomPodcastEpisodeListResponse : PodcastIndexResponseBase
    {
        public List<Episode> episodes { get; set; }
    }
}
