using System.Text.RegularExpressions;

namespace PaperlessREST.OcrWorker.Services
{
    public class DocumentSimilarityService : IDocumentSimilarityService
    {
        public double CalculateSimilarity(string text1, string text2)
        {
            var tokens1 = Tokenize(text1);
            var tokens2 = Tokenize(text2);

            var intersection = tokens1.Intersect(tokens2).Count();
            var union = tokens1.Union(tokens2).Count();

            return union == 0 ? 0 : (double)intersection / union;
        }

        private IEnumerable<string> Tokenize(string text)
        {
            return Regex
                .Split(text.ToLowerInvariant(), @"\W+")
                .Where(t => t.Length > 3);
        }
    }
}
