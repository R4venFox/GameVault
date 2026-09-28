using GameVault.Data.Models;

namespace GameVault.Business.Models;

public enum SortiranjeIgara
{
    Naziv,
    GodinaIzdanja,
    Ocena,
    BrojSati,
    DatumDodavanja
}

public class IgraPretraga
{
    public string? Tekst { get; set; }
    public StatusIgre? Status { get; set; }
    public int? ZanrId { get; set; }
    public int? PlatformaId { get; set; }
    public bool SamoOmiljene { get; set; }
    public SortiranjeIgara Sortiranje { get; set; }
    public bool Opadajuce { get; set; }
}
