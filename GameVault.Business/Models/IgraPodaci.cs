using GameVault.Data.Models;

namespace GameVault.Business.Models;

public class IgraPodaci
{
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int? GodinaIzdanja { get; set; }
    public string? Developer { get; set; }
    public string? Izdavac { get; set; }
    public StatusIgre Status { get; set; } = StatusIgre.Planirana;
    public int? Ocena { get; set; }
    public int BrojSati { get; set; }
    public bool Omiljena { get; set; }
    public List<int> ZanrIds { get; set; } = new();
    public List<int> PlatformaIds { get; set; } = new();
}
