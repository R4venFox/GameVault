using GameVault.Business.Models;

namespace GameVault.Web.Models;

public class StatistikaViewModel
{
    public int UkupnoIgara { get; set; }
    public int Planirane { get; set; }
    public int UToku { get; set; }
    public int Zavrsene { get; set; }
    public int Napustene { get; set; }
    public int Omiljene { get; set; }
    public long UkupnoSati { get; set; }
    public double? ProsecnaOcena { get; set; }
    public string? NajcesciZanr { get; set; }
    public string? NajcescaPlatforma { get; set; }

    public static StatistikaViewModel IzPodataka(StatistikaPodaci podaci) => new()
    {
        UkupnoIgara = podaci.UkupnoIgara,
        Planirane = podaci.Planirane,
        UToku = podaci.UToku,
        Zavrsene = podaci.Zavrsene,
        Napustene = podaci.Napustene,
        Omiljene = podaci.Omiljene,
        UkupnoSati = podaci.UkupnoSati,
        ProsecnaOcena = podaci.ProsecnaOcena,
        NajcesciZanr = podaci.NajcesciZanr,
        NajcescaPlatforma = podaci.NajcescaPlatforma
    };
}
