using System;

namespace Agfinn.PodcastIndex.Lib
{
    public class PodcastIndexResponseBase
    {
        public string status { get; set; }
        public int? count { get; set; }
        public object query { get; set; }
        public string description { get; set; }
    }
}
