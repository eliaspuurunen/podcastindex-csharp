using System;

namespace Agfinn.PodcastIndex.Lib
{
    public class Episode
    {
        public int id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public string guid { get; set; }
        public int datePublished { get; set; }
        public int dateCrawled { get; set; }
        public string enclosureUrl { get; set; }
        public string enclosureType { get; set; }
        public int enclosureLength { get; set; }
        public int _explicit { get; set; }
        public int episode { get; set; }
        public string episodeType { get; set; }
        public int season { get; set; }
        public string image { get; set; }
        public int feedItunesId { get; set; }
        public string feedImage { get; set; }
        public int feedId { get; set; }
        public string feedLanguage { get; set; }
        public string chaptersUrl { get; set; }
        public string transcriptUrl { get; set; }
    }
}
