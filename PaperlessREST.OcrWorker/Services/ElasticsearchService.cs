using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;

namespace PaperlessREST.OcrWorker.Services
{
    public class ElasticsearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly ILogger<ElasticsearchService> _logger;

        public ElasticsearchService(ILogger<ElasticsearchService> logger)
        {
            var settings = new ElasticsearchClientSettings(new Uri("http://elasticsearch:9200")).DefaultIndex("documents");
            _client = new ElasticsearchClient(settings);
            _logger = logger;
        }

        public async Task IndexDocumentAsync(Guid documentId, string title, string content){
            var doc = new
            {
                Id = documentId,
                Title = title,
                Content = content,
                IndexedAt = DateTime.UtcNow
            };

            var response = await _client.IndexAsync(doc, i => i.Id(documentId.ToString()));

            if (!response.IsValidResponse)
            {
                _logger.LogError("Elasticsearch indexing failed: {Reason}",
                    response.ElasticsearchServerError?.Error.Reason);
                throw new Exception("Indexing failed");
            }

            _logger.LogInformation("Document {Id} indexed in Elasticsearch", documentId);
        }
    }
}
