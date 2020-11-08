using System;

namespace Agfinn.PodcastIndex.Lib
{
    public class PodcastEpisodeResponse : PodcastIndexResponseBase
    {
        public Episode episode { get; set; }
    }
}
