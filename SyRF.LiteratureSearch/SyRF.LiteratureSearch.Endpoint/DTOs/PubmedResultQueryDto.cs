namespace SyRF.LiteratureSearch.Endpoint.DTOs
{
    public class PubmedResultQueryDto
    {
        public string? WebEnv { get; set; }
        public string? QueryKey { get; set; }
        public int Count { get; set; }
        
    }
}