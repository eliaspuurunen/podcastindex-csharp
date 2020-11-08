using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Agfinn.PodcastIndex.Lib
{
    public class PodcastFeed
    {
        public int id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string originalUrl { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public string ownerName { get; set; }
        public string image { get; set; }
        public string artwork { get; set; }
        public int lastUpdateTime { get; set; }
        public int lastCrawlTime { get; set; }
        public int lastParseTime { get; set; }
        public int lastGoodHttpStatusTime { get; set; }
        public int lastHttpStatus { get; set; }
        public string contentType { get; set; }
        public int itunesId { get; set; }
        public string generator { get; set; }
        public string language { get; set; }
        public int type { get; set; }
        public int dead { get; set; }
        public int crawlErrors { get; set; }
        public int parseErrors { get; set; }
        public object categories { get; set; }
        public int locked { get; set; }
        public int imageUrlHash { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Categories
        {
            get
            {
                if(this.categories is JArray)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<Dictionary<string, string>>((this.categories as JObject).ToString());
            }
        }

        public PodcastFeed()
        {

        }
    }
}
