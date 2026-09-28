using System.ComponentModel.DataAnnotations;
using GameVault.Business.Models;
using GameVault.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameVault.Web.Models;

public class IgraFormaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv igre je obavezan.")]
    [Display(Name = "Naziv")]
    public string Naziv { get; set; } = string.Empty;

    [Display(Name = "Opis")]
    public string? Opis { get; set; }

    [Display(Name = "Lične beleške")]
    public string? Beleske { get; set; }

    [Display(Name = "Godina izdanja")]
    public int? GodinaIzdanja { get; set; }

    [Display(Name = "Razvojni studio")]
    public string? Developer { get; set; }

    [Display(Name = "Izdavač")]
    public string? Izdavac { get; set; }

    [EnumDataType(typeof(StatusIgre), ErrorMessage = "Izaberite ispravan status.")]
    [Display(Name = "Status")]
    public StatusIgre Status { get; set; }

    [Range(1, 10, ErrorMessage = "Ocena mora biti od 1 do 10.")]
    [Display(Name = "Ocena")]
    public int? Ocena { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Broj sati ne sme biti negativan.")]
    [Display(Name = "Broj sati")]
    public int BrojSati { get; set; }

    [Display(Name = "Omiljena igra")]
    public bool Omiljena { get; set; }

    [Display(Name = "Žanrovi")]
    public List<int> ZanrIds { get; set; } = new();

    [Display(Name = "Platforme")]
    public List<int> PlatformaIds { get; set; } = new();

    [BindNever, ValidateNever]
    public List<SelectListItem> Zanrovi { get; set; } = new();

    [BindNever, ValidateNever]
    public List<SelectListItem> Platforme { get; set; } = new();

    public IgraPodaci UPoslovnePodatke() => new()
    {
        Naziv = Naziv, Opis = Opis, Beleske = Beleske, GodinaIzdanja = GodinaIzdanja,
        Developer = Developer, Izdavac = Izdavac, Status = Status,
        Ocena = Ocena, BrojSati = BrojSati, Omiljena = Omiljena,
        ZanrIds = ZanrIds, PlatformaIds = PlatformaIds
    };

    public static IgraFormaViewModel IzIgre(Igra igra) => new()
    {
        Id = igra.Id, Naziv = igra.Naziv, Opis = igra.Opis, Beleske = igra.Beleske,
        GodinaIzdanja = igra.GodinaIzdanja, Developer = igra.Developer,
        Izdavac = igra.Izdavac, Status = igra.Status, Ocena = igra.Ocena,
        BrojSati = igra.BrojSati, Omiljena = igra.Omiljena,
        ZanrIds = igra.Zanrovi.Select(z => z.Id).ToList(),
        PlatformaIds = igra.Platforme.Select(p => p.Id).ToList()
    };
}
