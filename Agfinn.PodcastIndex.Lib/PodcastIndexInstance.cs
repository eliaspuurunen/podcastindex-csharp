using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Agfinn.PodcastIndex.Lib
{
    /// <summary>
    /// Represents a class used to query the Podcast Index API.
    /// </summary>
    public class PodcastIndexInstance
    {
        private string userAgent = "Generic PodcastIndexInstance";
        private string apiKey;
        private string apiSecret;
        private const string PODCAST_INDEX_BASE_URI = "https://api.podcastindex.org";

        private IRestClient restClient;
        protected IRestClient RestClient
        {
            get
            {
                if (this.restClient == null)
                {
                    this.restClient = this.GetRestClient();
                }

                return this.restClient;
            }
        }

        /// <summary>
        /// Build a new instance of the API wrapper.
        /// </summary>
        /// <param name="apiKey">Your API key from Podcast Index.</param>
        /// <param name="apiSecret">You secret key from Podcast Index.</param>
        /// <param name="userAgent">Optional - the useragent each request should use.</param>
        public PodcastIndexInstance(
            string apiKey,
            string apiSecret,
            string userAgent = null)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.userAgent = this.userAgent ?? userAgent;
        }

        private IRestRequest PrepareRequest(string uriEndpoint, Method type)
        {
            var request = new RestRequest(uriEndpoint, type);
            // Adapted from sample code.
            // https://podcastindex-org.github.io/docs-api/
            var hash = string.Empty;

            var unixTimeStamp = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var apiHeaderTime = (int)unixTimeStamp.TotalSeconds;

            using (var sha1 = new SHA1Managed())
            {
                var hashed = sha1.ComputeHash(Encoding.UTF8.GetBytes(this.apiKey + this.apiSecret + apiHeaderTime));
                var sb = new StringBuilder(hashed.Length * 2);

                foreach (byte b in hashed)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                hash = sb.ToString();
            }

            request.AddHeader("X-Auth-Date", apiHeaderTime.ToString());
            request.AddHeader("X-Auth-Key", this.apiKey);
            request.AddHeader("Authorization", hash);
            return request;
        }

        /// <summary>
        /// Calls the /api/1.0/search/byterm API route. Search terms will be
        /// automatically concatenated and escaped.
        /// </summary>
        /// <param name="searchTerms">The search terms to use. Spaces will be added for you.</param>
        /// <returns>Returns a collection of podcast feeds. May be null.</returns>
        public async Task<ICollection<PodcastFeed>> SearchByTermAsync(params string[] searchTerms)
        {
            var request = this.PrepareRequest("/api/1.0/search/byterm", Method.GET);
            request.AddQueryParameter("q", string.Join(" ", searchTerms), true);
            var response = await this.RestClient.GetAsync<SearchResponse>(request);

            try
            {
                return response?.feeds;                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a podcast feed by its URL. Calls the /api/1.0/podcasts/byfeedurl route. 
        /// </summary>
        /// <param name="feedUrl">The feed URL to use. Does not need to be escaped.</param>
        /// <returns>Returns a specific podcast feed. Can be null.</returns>
        public async Task<PodcastFeed> GetFeedAsync(string feedUrl)
        {
            var request = this.PrepareRequest("/api/1.0/podcasts/byfeedurl", Method.GET);
            request.AddQueryParameter("url", feedUrl, false);
            var response = await this.RestClient.GetAsync<FeedResponse>(request);

            try
            {
                return response?.feed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a podcast feed by its ID or by iTunes ID. Calls /api/1.0/podcasts/byfeedid.
        /// </summary>
        /// <param name="feedId">The feed ID or iTunes ID of the feed.</param>
        /// <param name="iTunesMode">If true, search is done by iTunes feed ID.</param>
        /// <returns>Returns a specific podcast feed. May be null.</returns>
        public async Task<PodcastFeed> GetFeedAsync(int feedId, bool iTunesMode = false)
        {
            var request = this.PrepareRequest($"/api/1.0/podcasts/{(iTunesMode ? "byitunesid" : "byfeedid")}", Method.GET);
            request.AddQueryParameter("id", feedId.ToString(), false);
            var response = await this.RestClient.GetAsync<FeedResponse>(request);

            try
            {
                return response?.feed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the episodes for a specific podcast via the podcast feed URL. Calls /api/1.0/episodes/byfeedurl.
        /// </summary>
        /// <param name="feedUrl">The podcast URL to search.</param>
        /// <param name="max">The maximum number of episodes to retrieve. Default is 10.</param>
        /// <param name="since">Optional - retrieve episodes since a specified date.</param>
        /// <returns>Returns a list of episodes. May be null or empty.</returns>
        public async Task<List<Episode>> GetEpisodesForFeedAsync(
            string feedUrl,
            int max = 10,
            DateTimeOffset? since = null)
        {
            var request = this.PrepareRequest("/api/1.0/episodes/byfeedurl", Method.GET);
            request.AddQueryParameter("url", feedUrl, false);

            if(max <= 0 && max > 40)
            {
                max = 10;
            }

            request.AddParameter("max", max);

            if(since != null)
            {
                request.AddQueryParameter("since", since.Value.ToUnixTimeSeconds().ToString());
            }

            var response = await this.RestClient.GetAsync<PodcastEpisodeListResponse>(request);

            try
            {
                return response?.items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the episodes for a specific podcast via the podcast ID or iTunes ID. Calls /api/1.0/episodes/byfeedid.
        /// </summary>
        /// <param name="feedId">The podcast ID/iTunes ID to search.</param>
        /// <param name="max">The maximum number of episodes to retrieve. Default is 10.</param>
        /// <param name="since">Optional - retrieve episodes since a specified date.</param>
        /// <param name="iTunesMode">Optional - if true, search by iTunes ID.</param>
        /// <returns>Returns a list of episodes. May be null or empty.</returns>
        public async Task<List<Episode>> GetEpisodesForFeedAsync(
            int feedId,
            int max = 10,
            DateTimeOffset? since = null,
            bool iTunesMode = false)
        {
            var request = this.PrepareRequest($"/api/1.0/episodes/{(iTunesMode ? "byitunesid" : "byfeedid")}", Method.GET);
            request.AddQueryParameter("id", feedId.ToString());

            if (max <= 0 && max > 40)
            {
                max = 10;
            }

            request.AddParameter("max", max);

            if (since != null)
            {
                request.AddQueryParameter("since", since.Value.ToUnixTimeSeconds().ToString());
            }

            var response = await this.RestClient.GetAsync<PodcastEpisodeListResponse>(request);

            try
            {
                return response?.items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a specific episode by podcast episode ID. Calls /api/1.0/episodes/byid
        /// </summary>
        /// <param name="episodeId">The episode ID to retrieve.</param>
        /// <returns>Returns a specific episode. May be null.</returns>
        public async Task<Episode> GetEpisodeAsync(int episodeId)
        {
            var request = this.PrepareRequest("/api/1.0/episodes/byid", Method.GET);
            request.AddQueryParameter("id", episodeId.ToString(), false);
            var response = await this.RestClient.GetAsync<PodcastEpisodeResponse>(request);

            try
            {
                return response?.episode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a random set of episodes, based on the options specified. Calls /api/1.0/episodes/random
        /// </summary>
        /// <param name="options">The search options to use.</param>
        /// <returns>A list of episodes. May be null/empty.</returns>
        public async Task<List<Episode>> GetRandomEpisodesAsync(SearchOptions options)
        {
            var request = this.PrepareRequest("/api/1.0/episodes/random", Method.GET);

            if(options.Maximum < 1 || options.Maximum > 40)
            {
                options.Maximum = 1;
            }

            request.AddQueryParameter("max", options.Maximum.ToString());

            if (options.Language.Any())
            {
                request.AddQueryParameter("lang", string.Join(",", options.Language));
            }

            if (options.Categories.Any())
            {
                request.AddQueryParameter("cat", string.Join(",", options.Categories));
            }

            if (options.ExcludeCategories.Any())
            {
                request.AddQueryParameter("notcat", string.Join(",", options.ExcludeCategories));
            }

            var response = await this.RestClient.GetAsync<RandomPodcastEpisodeListResponse>(request);

            try
            {
                return response?.episodes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves recent episodes from Podcast Index's global list. Calls /api/1.0/recent/episodes.
        /// </summary>
        /// <param name="max">The maximum number of episodes to retrieve. Default is 10.</param>
        /// <param name="excludeString">The search string to exclude.</param>
        /// <param name="beforeEpisodeId">If specified, you will get recent episodes before the specified ID.</param>
        /// <returns>Returns a list of episodes. May be null/empty.</returns>
        public async Task<List<Episode>> GetRecentEpisodesAsync(
            int max = 10,
            string excludeString = null,
            int? beforeEpisodeId = null)
        {
            var request = this.PrepareRequest("/api/1.0/recent/episodes", Method.GET);

            if(max < 0 || max > 100)
            {
                max = 10;
            }

            request.AddQueryParameter("max", max.ToString());

            if (!string.IsNullOrEmpty(excludeString))
            {
                request.AddQueryParameter("excludeString", excludeString, false);
            }

            if(beforeEpisodeId != null)
            {
                request.AddQueryParameter("before", beforeEpisodeId.ToString(), true);
            }

            var response = await this.RestClient.GetAsync<PodcastEpisodeListResponse>(request);

            try
            {
                return response?.items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves recent global podcasts, based on the options specified. Calls /api/1.0/recent/feeds
        /// </summary>
        /// <param name="options">The search options to use.</param>
        /// <returns>A list of podcast feeds. May be null/empty.</returns>
        public async Task<List<PodcastFeed>> GetRecentFeedsAsync(SearchOptions options)
        {
            var request = this.PrepareRequest("/api/1.0/recent/feeds", Method.GET);

            if (options.Maximum < 1 || options.Maximum > 40)
            {
                options.Maximum = 1;
            }

            request.AddQueryParameter("max", options.Maximum.ToString());

            if (options.Language.Any())
            {
                request.AddQueryParameter("lang", string.Join(",", options.Language));
            }

            if (options.Categories.Any())
            {
                request.AddQueryParameter("cat", string.Join(",", options.Categories));
            }

            if (options.ExcludeCategories.Any())
            {
                request.AddQueryParameter("notcat", string.Join(",", options.ExcludeCategories));
            }

            var response = await this.RestClient.GetAsync<SearchResponse>(request);

            try
            {
                return response?.feeds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the newest podcast feeds over the past 24 hours. Calls /api/1.0/recent/newfeeds.
        /// </summary>
        /// <returns>Returns a list of podcast feeds. May be null/empty.</returns>
        public async Task<List<PodcastFeed>> GetNewFeedsAsync()
        {
            var request = this.PrepareRequest("/api/1.0/recent/newfeeds", Method.GET);

            var response = await this.RestClient.GetAsync<SearchResponse>(request);

            try
            {
                return response?.feeds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RestClient GetRestClient()
        {
            var toReturn = new RestClient(PODCAST_INDEX_BASE_URI);
            toReturn.UserAgent = this.userAgent;
            toReturn.UseNewtonsoftJson();
            return toReturn;
        }
    }
}
