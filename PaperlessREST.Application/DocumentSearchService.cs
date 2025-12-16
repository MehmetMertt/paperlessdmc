using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class DocumentSearchService
{
    private readonly ElasticsearchClient _client;

    public DocumentSearchService()
    {
        var settings = new ElasticsearchClientSettings(new Uri("http://elasticsearch:9200")).DefaultIndex("documents");
        _client = new ElasticsearchClient(settings);
    }

    public async Task<IReadOnlyCollection<dynamic>> SearchAsync(string query)
    {
        var lowered = query.ToLowerInvariant();

        var response = await _client.SearchAsync<dynamic>(s => s
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sh => sh.Wildcard(w => w.Field("id.keyword").Value($"*{lowered}*")),    // ID (GUID, substrings)
                        sh => sh.Wildcard(w => w.Field("title.keyword").Value($"*{lowered}*")), // filename and its substrings (eg. file, filename, filname123)
                        sh => sh.Match(m => m.Field("type").Query(query)),
                        sh => sh.Match(m => m.Field("content").Query(query))
                    )
                )
            )
        );

        return response.Documents;
    }
}
