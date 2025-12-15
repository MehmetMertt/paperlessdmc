namespace PaperlessREST.OcrWorker.Services
{
    public interface IDocumentSimilarityService
    {
        double CalculateSimilarity(string text1, string text2);
    }
}
