using Godot;

public enum Car
{
    Nicht_Fahrer,
    Kangoo_MYK__TS_178,
    Fiat_Klein_MYK__TS_278,
    Fiat_Bus_MYK__TS_378
}

public static class CarNames
{
    public static string GetName(Car car) => car.ToString().ReplaceN("__", "-").ReplaceN("_", " ");
}