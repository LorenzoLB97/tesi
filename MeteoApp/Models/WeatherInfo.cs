public class WeatherInfo
{
    public Main Main { get; set; }
    public Weather[] Weather { get; set; }
    public Wind Wind { get; set; }
}

public class Main
{
    public float Temp { get; set; }
    public float Feels_Like { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
}

public class Weather
{
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class Wind
{
    public float Speed { get; set; }
    public int Deg { get; set; }
}
