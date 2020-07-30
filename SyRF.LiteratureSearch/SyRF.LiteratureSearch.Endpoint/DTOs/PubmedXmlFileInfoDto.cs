namespace SyRF.LiteratureSearch.Endpoint.DTOs
{
    public class PubmedXmlFileInfoDto
    {
        public PubmedXmlFileInfoDto(string fileUri, string webEnv, int totalNumberOfReference,
            int fileNumber, int numberOfReference, int totalFileNumber, string queryKey)
        {
            FileUri = fileUri;
            WebEnv = webEnv;
            TotalNumberOfReferences = totalNumberOfReference;
            FileNumber = fileNumber;
            NumberOfReferences = numberOfReference;
            TotalFileNumber = totalFileNumber;
            QueryKey = queryKey;
        }

        public string QueryKey { get; }
        public int TotalFileNumber { get; }
        public string WebEnv { get; }
        public int TotalNumberOfReferences { get; }
        public int FileNumber { get; }
        public int NumberOfReferences { get; }
        public string FileUri { get; }
    }
}