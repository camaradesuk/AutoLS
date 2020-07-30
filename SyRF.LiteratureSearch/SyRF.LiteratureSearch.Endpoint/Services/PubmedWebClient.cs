using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SyRF.LiteratureSearch.Endpoint.DTOs;
using SyRF.LiteratureSearch.Endpoint.Interfaces;

namespace SyRF.LiteratureSearch.Endpoint.Services
{
    public class PubmedWebClient : IPubmedWebClient
    {
        private readonly HttpClient _client;

        public PubmedWebClient(HttpClient client)
        {
            _client = client;
        }

        private async Task<string> PostRequestToPubmed(string link, Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);
            System.Threading.Thread.Sleep(1000);
            try
            {
                var response = await _client.PostAsync(link, content);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine($"Error while getting studies from PubMed. Error: {e.Message}");
                return $"Cannot get studies from PubMed. Error: {e.Message}";
            }
        }

        public async Task<PubmedResultQueryDto> SubmitSearch(string searchQuery)
        {
            const string searchLink = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi";
            var values = new Dictionary<string, string>
            {
                {"db", "pubmed"},
                {"term", searchQuery},
                {"usehistory", "y"},
                {"retmode", "json"}
            };
            var result = await PostRequestToPubmed(searchLink, values);

            var obj = JObject.Parse(result);
            var webEnv = obj["esearchresult"]?["webenv"]?.ToString();
            var queryKey = obj["esearchresult"]?["querykey"]?.ToString();
            var count = int.Parse(obj["esearchresult"]?["count"]?.ToString() ?? string.Empty);
            return new PubmedResultQueryDto {WebEnv = webEnv, QueryKey = queryKey, Count = count};
        }

        public async Task<string> GetRecordsXmlString(string? webEnv, string? queryKey, int batchSize, int retStart = 0)
        {
            const string fetchLink = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi";
            var values = new Dictionary<string, string>
            {
                {"db", "pubmed"},
                {"webEnv", webEnv},
                {"query_key", queryKey},
                {"retstart", retStart.ToString()},
                {"retmax", batchSize.ToString()},
                {"retmode", "xml"},
            };
            var result = await PostRequestToPubmed(fetchLink, values);
            return result;
        }
    }
}