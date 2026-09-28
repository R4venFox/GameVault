using GameVault.Business.Models;
using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Business.Services;

public class StatistikaService : IStatistikaService
{
    private readonly IIgraRepository igre;

    public StatistikaService(IIgraRepository igre)
    {
        this.igre = igre;
    }

    public async Task<StatistikaPodaci> DohvatiAsync(CancellationToken cancellationToken = default)
    {
        var kolekcija = await igre.DohvatiSveAsync(true, cancellationToken);
        return new StatistikaPodaci
        {
            UkupnoIgara = kolekcija.Count,
            Planirane = kolekcija.Count(i => i.Status == StatusIgre.Planirana),
            UToku = kolekcija.Count(i => i.Status == StatusIgre.UToku),
            Zavrsene = kolekcija.Count(i => i.Status == StatusIgre.Zavrsena),
            Napustene = kolekcija.Count(i => i.Status == StatusIgre.Napustena),
            Omiljene = kolekcija.Count(i => i.Omiljena),
            UkupnoSati = kolekcija.Sum(i => (long)i.BrojSati),
            ProsecnaOcena = kolekcija.Average(i => (double?)i.Ocena),
            NajcesciZanr = kolekcija.SelectMany(i => i.Zanrovi.DistinctBy(z => z.Id))
                .GroupBy(z => z.Id).OrderByDescending(g => g.Count())
                .ThenBy(g => g.First().Naziv, StringComparer.OrdinalIgnoreCase).ThenBy(g => g.Key)
                .Select(g => g.First().Naziv).FirstOrDefault(),
            NajcescaPlatforma = kolekcija.SelectMany(i => i.Platforme.DistinctBy(p => p.Id))
                .GroupBy(p => p.Id).OrderByDescending(g => g.Count())
                .ThenBy(g => g.First().Naziv, StringComparer.OrdinalIgnoreCase).ThenBy(g => g.Key)
                .Select(g => g.First().Naziv).FirstOrDefault()
        };
    }
}
