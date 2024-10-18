namespace MeteoApp;

[QueryProperty(nameof(Entry), "Entry")]
public partial class MeteoItemPage : ContentPage
{
    Entry entry;
    public Entry Entry
    {
        get => entry;
        set
        {
            entry = value;
            OnPropertyChanged();
        }
    }

    public MeteoItemPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    /**
     * Metodo di ContentPage
     * viene eseguito ogni volta che una pagina in un'applicazione 
     * .NET MAUI diventa visibile all'utente
     * Questo è utile in molti scenari, come:
     * Aggiornare i dati della pagina prima che venga mostrata.
     * Avviare processi, come recuperare dati da un'API o avviare animazioni.
     * Registrare eventi o gestire l'interfaccia utente che dipende dalla visibilità della pagina.
     */
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}