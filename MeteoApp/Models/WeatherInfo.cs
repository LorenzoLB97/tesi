using System.Text.Json.Serialization;

public class WeatherInfo
{
    [JsonPropertyName("coord")]
    public Coord Coord { get; set; }

    [JsonPropertyName("weather")]
    public Weather[] Weather { get; set; }

    [JsonPropertyName("base")]
    public string Base { get; set; }

    [JsonPropertyName("main")]
    public Main Main { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }

    [JsonPropertyName("rain")]
    public Rain Rain { get; set; }

    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("sys")]
    public Sys Sys { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("cod")]
    public int Cod { get; set; }

    public override string ToString()
    {
        return $"WeatherInfo: \n" +
               $"Coord: {(Coord != null ? Coord.ToString() : "N/A")}\n" +
               $"Weather: {(Weather != null ? string.Join(", ", Weather.Select(w => w.ToString())) : "N/A")}\n" +
               $"Base: {Base ?? "N/A"}\n" +
               $"Main: {(Main != null ? Main.ToString() : "N/A")}\n" +
               $"Visibility: {Visibility}\n" +
               $"Wind: {(Wind != null ? Wind.ToString() : "N/A")}\n" +
               $"Rain: {(Rain != null ? Rain.ToString() : "N/A")}\n" +
               $"Clouds: {(Clouds != null ? Clouds.ToString() : "N/A")}\n" +
               $"Date Time (Unix): {Dt}\n" +
               $"Sys: {(Sys != null ? Sys.ToString() : "N/A")}\n" +
               $"Timezone: {Timezone}\n" +
               $"Id: {Id}\n" +
               $"Name: {Name ?? "N/A"}\n" +
               $"Cod: {Cod}";
    }
}

public class Coord
{
    [JsonPropertyName("lon")]
    public float Lon { get; set; }

    [JsonPropertyName("lat")]
    public float Lat { get; set; }

    public override string ToString()
    {
        return $"Coord [Longitude: {Lon}, Latitude: {Lat}]";
    }
}


public class Main
{
    private float _temp;
    private float _feelsLike;
    private float _tempMin;
    private float _tempMax;

    [JsonPropertyName("temp")]
    public float Temp
    {
        get => _temp;
        set => _temp = value;
    }

    // Proprietà per avere la temperatura in Celsius
    public float TempCelsius => _temp - 273.15f;

    [JsonPropertyName("feels_like")]
    public float FeelsLike
    {
        get => _feelsLike;
        set => _feelsLike = value;
    }

    public float FeelsLikeCelsius => _feelsLike - 273.15f;

    [JsonPropertyName("temp_min")]
    public float TempMin
    {
        get => _tempMin;
        set => _tempMin = value;
    }

    public float TempMinCelsius => _tempMin - 273.15f;

    [JsonPropertyName("temp_max")]
    public float TempMax
    {
        get => _tempMax;
        set => _tempMax = value;
    }

    public float TempMaxCelsius => _tempMax - 273.15f;

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("sea_level")]
    public int SeaLevel { get; set; }

    [JsonPropertyName("grnd_level")]
    public int GrndLevel { get; set; }

    // ToString aggiornato per mostrare le temperature in gradi Celsius
    public override string ToString()
    {
        return $"Main [Temperature (Celsius): {TempCelsius}, Feels Like (Celsius): {FeelsLikeCelsius}, " +
               $"Min Temp (Celsius): {TempMinCelsius}, Max Temp (Celsius): {TempMaxCelsius}, " +
               $"Pressure: {Pressure}, Humidity: {Humidity}, Sea Level: {SeaLevel}, Ground Level: {GrndLevel}]";
    }
}




public class Weather
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string MainDescription { get; set; } // Rinominato per evitare conflitti con la classe Main

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    public override string ToString()
    {
        return $"Weather [Id: {Id}, Main: {MainDescription}, Description: {Description}, Icon: {Icon}]";
    }
}


public class Wind
{
    [JsonPropertyName("speed")]
    public float Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Deg { get; set; }

    [JsonPropertyName("gust")]
    public float Gust { get; set; }

    public override string ToString()
    {
        return $"Wind [Speed: {Speed}, Degree: {Deg}, Gust: {Gust}]";
    }
}


public class Rain
{
    [JsonPropertyName("1h")]
    public float OneHour { get; set; }

    public override string ToString()
    {
        return $"Rain [1h: {OneHour}]";
    }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int All { get; set; }

    public override string ToString()
    {
        return $"Clouds [All: {All}%]";
    }
}


public class Sys
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }

    public override string ToString()
    {
        return $"Sys [Type: {Type}, Id: {Id}, Country: {Country}, Sunrise (Unix): {Sunrise}, Sunset (Unix): {Sunset}]";
    }
}

