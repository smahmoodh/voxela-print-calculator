namespace Voxela3D.PrintCalculator;

public sealed class Filament
{
    public string Name { get; set; } = "PLA";
    public string Brand { get; set; } = "";
    public string Type { get; set; } = "PLA";
    public decimal Price { get; set; } = 500000;
    public string Unit { get; set; } = "kg";
    public override string ToString() => $"{Name} — {Brand} ({Type})";
}

public sealed class AppSettings
{
    public decimal HourlyRate { get; set; } = 110000m;
    public decimal ProfitPercent { get; set; } = 30m;
    public List<Filament> Filaments { get; set; } =
    [
        new() { Name = "PLA+", Brand = "eSUN", Type = "PLA+", Price = 5400000m, Unit = "kg" }
    ];
}