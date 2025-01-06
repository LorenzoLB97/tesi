using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using Microsoft.Extensions.Configuration;

namespace MeteoApp.Services
{
    public class AppwriteService
    {
        private readonly Client _client;
        private readonly Databases _databases; //salviamo il riferimento a databases

        private readonly string ApiKey; 
        private readonly string ProjectApiKey; 
        private readonly string DatabaseId; 
        private readonly string CollectionId; 

        public AppwriteService(IConfiguration configuration) {
            _client = new Client();

            ApiKey = configuration["AppwriteApiKey"];
            ProjectApiKey = configuration["AppwriteProjectApiKey"];
            DatabaseId = configuration["AppwriteDatabaseId"];
            CollectionId = configuration["AppwriteCollectionId"];

            _client
                 .SetEndpoint("https://cloud.appwrite.io/v1")
                 .SetProject(ProjectApiKey)
                 .SetKey(ApiKey); // Sostituisci con la tua API Key

            _databases = new Databases(_client);
        }

        public async Task CreateAppwriteDB()
        {
            try
            {
                // Recupera il database esistente
                var existingDatabase = await _databases.List(); // Ottieni l'elenco dei database
                if (!existingDatabase.Databases.Any(db => db.Id == DatabaseId))
                {
                    // Crea un nuovo database solo se non esiste
                    await _databases.Create(databaseId: DatabaseId, name: "TodosDB");
                    Console.WriteLine("Database creato con successo.");                    
                }
                else
                {
                    Console.WriteLine("Il database esiste già. Nessuna creazione necessaria.");
                }

                // Recupera la collezione esistente
                var collections = await _databases.ListCollections(DatabaseId);
                if (!collections.Collections.Any(c => c.Id == CollectionId))
                {
                    // Crea una nuova collezione solo se non esiste
                    await _databases.CreateCollection(
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
                    Console.WriteLine("Collezione creata con successo.");
                }
                else
                {
                    Console.WriteLine("La collezione esiste già. Nessuna creazione necessaria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la creazione del database o collezione: {ex.Message}");
            }
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

                Debug.WriteLine("Entry salvata con successo in Appwrite:");
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore durante il salvataggio dell'entry in Appwrite:");
                Debug.WriteLine(ex.Message);
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
                        $"equal(\"latitude\", {entry.Latitude})",
                        $"equal(\"longitude\", {entry.Longitude})"
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

        public async Task appWriteTestConnection()
        {
            try
            {
                var response = await _databases.List();
                Debug.WriteLine($"Connessione riuscita! Trovati {response.Databases.Count} database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore durante la connessione ad Appwrite:");
                Debug.WriteLine(ex.Message);
            }
        }

        //da testare FIXARE
        public async Task<List<Entry>> GetAllEntriesAsync()
        {
            var entries = new List<Entry>();

            try
            {
                // Recupera tutti i documenti dalla collezione di Appwrite
                var documents = await _databases.ListDocuments(
                    databaseId: DatabaseId,
                    collectionId: CollectionId
                );

                foreach (var document in documents.Documents)
                {
                    // Crea un'istanza di Entry usando i dati del documento
                    var entry = new Entry
                    {
                        Id = int.TryParse(document.Id, out int id) ? id : 0,
                        CompleteAddress = document.Data["completeAddress"]?.ToString(),
                        Street = document.Data["street"]?.ToString(),
                        City = document.Data["city"]?.ToString(),
                        PostalCode = document.Data["postalCode"]?.ToString(),
                        Country = document.Data["country"]?.ToString(),
                        Latitude = double.TryParse(document.Data["latitude"]?.ToString(), out double lat) ? lat : 0.0,
                        Longitude = double.TryParse(document.Data["longitude"]?.ToString(), out double lng) ? lng : 0.0,
                        IsCurrentLocation = bool.TryParse(document.Data["isCurrentLocation"]?.ToString(), out bool isCurrent) ? isCurrent : false
                    };

                    entries.Add(entry);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante il recupero delle entries da Appwrite: {ex.Message}");
            }

            return entries;
        }

    }
}
