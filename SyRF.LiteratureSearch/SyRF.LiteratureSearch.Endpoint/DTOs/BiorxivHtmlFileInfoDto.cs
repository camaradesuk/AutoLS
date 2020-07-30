namespace SyRF.LiteratureSearch.Endpoint.DTOs
{
    public class BiorxivHtmlFileInfoDto
    {
        public BiorxivHtmlFileInfoDto(string fileUri, int fileNumber, int numberOfReferences)
        {
            FileUri = fileUri;
            FileNumber = fileNumber;
            NumberOfReferences = numberOfReferences;
        }

        public string FileUri { get; set; }
        public int FileNumber { get; set; }
        public int NumberOfReferences { get; set; }
    }
}