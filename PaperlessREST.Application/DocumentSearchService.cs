using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

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
        var response = await _client.SearchAsync<dynamic>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(new[] { new Field("title"), new Field("content"), new Field("type") })
                    .Query(query)
                )
            )
        );

        if (!response.IsValidResponse)
        {
            Console.WriteLine("Elasticsearch error:");
            Console.WriteLine(response.ElasticsearchServerError?.Error.Reason);
            return Array.Empty<dynamic>();
        }

        Console.WriteLine(
            "DOCUMENTS: " +
            System.Text.Json.JsonSerializer.Serialize(response.Documents)
        );

        return response.Documents;
    }
}
