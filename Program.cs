using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;

namespace TestBugDistinct
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var endpoint = config["CosmosDB:Endpoint"];
            var authKey = config["CosmosDB:AuthKey"];
            var databaseId = config["CosmosDB:DatabaseId"];
            var collectionId = config["CosmosDB:CollectionId"];

            var connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Https
            };

            using (var client = new DocumentClient(new Uri(endpoint), authKey, connectionPolicy))
            {
                var sql = @"SELECT DISTINCT VALUE c
        FROM c
        JOIN i in c.ExternalIdentities
        where c._documentType = 'user' and i.ProviderName = @provider and i.ProviderSubjectId = @providerSubjectId";

                var parameters = new SqlParameterCollection
                {
                    new SqlParameter("@provider", "AzureAD"),
                    new SqlParameter("@providerSubjectId", "some value"),
                };

                var querySpec = new SqlQuerySpec(sql, parameters);
                
                var collection = (await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId))).Resource;
                var query = client.CreateDocumentQuery<Document>(
                    collection.SelfLink,
                    querySpec,
                    new FeedOptions { EnableCrossPartitionQuery = true })
                    .AsDocumentQuery();

                int nResult = 0;
                while (query.HasMoreResults)
                {
                    foreach (var doc in await query.ExecuteNextAsync<Document>())
                    {
                        Console.WriteLine($"Result {nResult++}");
                        Console.WriteLine(doc.ToString());
                    }
                }
            }
        }
    }
}
