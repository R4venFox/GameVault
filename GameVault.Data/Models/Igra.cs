using System.ComponentModel.DataAnnotations;

namespace GameVault.Data.Models;

public class Igra
{
    public int Id { get; set; }

    [Required]
    public string Naziv { get; set; } = string.Empty;

    public string? Opis { get; set; }
    public int? GodinaIzdanja { get; set; }
    public string? Developer { get; set; }
    public string? Izdavac { get; set; }

    [EnumDataType(typeof(StatusIgre))]
    public StatusIgre Status { get; set; } = StatusIgre.Planirana;

    [Range(1, 10)]
    public int? Ocena { get; set; }

    [Range(0, int.MaxValue)]
    public int BrojSati { get; set; }

    public bool Omiljena { get; set; }
    public DateTime DatumDodavanja { get; set; } = DateTime.UtcNow;

    public ICollection<Zanr> Zanrovi { get; set; } = new List<Zanr>();
    public ICollection<Platforma> Platforme { get; set; } = new List<Platforma>();
}
