using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;

namespace MeteoApp.Services
{
    internal class AppwriteService
    {
        private readonly Client _client;
        private readonly Databases _databases; //salviamo il riferimento a databases

        private readonly string ApiKey = "standard_570f8b7f387f1c19de38d78e0126dc7cfc400959117e985d6e2c22211af2f7bceeffbd1f8ce5fe632267de44d31dc23a9b96da3d58c81e9c59596dfd98989109dacad12dfec8b6924826f5b3026139442fed0260d60c3e55fafad0dc0e7735e5659916110f13e4fabd3f5f8d28d408658d0af19644de30057dc1368397c19587";
        private const string ProjectApiKey = "674667ef0002958e47a4"; // Sostituisci con la tua API key
        private const string DatabaseId = "67472720001d3bc171b3"; // Sostituisci con il tuo ID database (presi dalla console)
        private const string CollectionId = "6747274b00139bc5a5fb"; // Sostituisci con il tuo ID collezione (presi dalla console)

        public AppwriteService() {
            _client = new Client();

            _client
                 .SetEndpoint("https://cloud.appwrite.io/v1")
                 .SetProject(ProjectApiKey)
                 .SetKey(ApiKey); // Sostituisci con la tua API Key

            _databases = new Databases(_client);
        }

        public async Task CreateAppwriteDB()
        {
            Database todoDatabase;
            Collection todoCollection;

            await _databases.Create(databaseId: DatabaseId, name: "TodosDB");

            todoCollection = await _databases.CreateCollection(
                databaseId: DatabaseId,
                collectionId: CollectionId,
                name: "Todos"
            );

            await _databases.CreateStringAttribute(
                databaseId: DatabaseId,
                collectionId: CollectionId,
                key: "title",
                size: 255,
                required: true
            );
        }

        public async Task SaveEntryAsync(Entry entry)
        {
            try
            {
                // Converti l'oggetto Entry in un dizionario
                var data = new Dictionary<string, object>
                {
                    { "CompleteAddress", entry.CompleteAddress },
                    { "Street", entry.Street },
                    { "City", entry.City },
                    { "PostalCode", entry.PostalCode },
                    { "Country", entry.Country },
                    { "Latitude", entry.Latitude },
                    { "Longitude", entry.Longitude },
                    { "IsCurrentLocation", entry.IsCurrentLocation }
                };

                // Salva l'entry nella collezione
                var result = await _databases.CreateDocument(
                    databaseId: DatabaseId,
                    collectionId: CollectionId,
                    documentId: ID.Unique(), // Genera automaticamente un ID univoco
                    data: data
                );

                Console.WriteLine("Entry salvata con successo in Appwrite:");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante il salvataggio dell'entry in Appwrite:");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteEntryAsync(string documentId)
        {
            try
            {
                // Elimina il documento
                await _databases.DeleteDocument(
                    databaseId: DatabaseId,
                    collectionId: CollectionId,
                    documentId: documentId
                );

                Console.WriteLine($"Documento con ID {documentId} eliminato con successo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante l'eliminazione del documento:");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteEntryAsync(Entry entry)
        {
            try
            {
                // Cerca il documento corrispondente utilizzando Latitude e Longitude
                var documents = await _databases.ListDocuments(
                    databaseId: DatabaseId,
                    collectionId: CollectionId,
                    queries: new List<string>
                    {
                        $"equal(\"Latitude\", {entry.Latitude})",
                        $"equal(\"Longitude\", {entry.Longitude})"
                    }
                );

                if (documents.Total == 0)
                {
                    Console.WriteLine("Nessun documento trovato per l'Entry specificata.");
                    return;
                }

                // Supponiamo che Latitude e Longitude identifichino un documento univoco
                var documentId = documents.Documents.First().Id;

                // Elimina il documento
                await _databases.DeleteDocument(
                    databaseId: DatabaseId,
                    collectionId: CollectionId,
                    documentId: documentId
                );

                Console.WriteLine($"Documento con ID {documentId} eliminato con successo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante l'eliminazione del documento:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
