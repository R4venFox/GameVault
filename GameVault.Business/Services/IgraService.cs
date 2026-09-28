using GameVault.Business.Models;
using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Business.Services;

public class IgraService : IIgraService
{
    public async Task<List<Igra>> PretraziAsync(IgraPretraga pretraga, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(pretraga.Sortiranje) ||
            (pretraga.Status.HasValue && !Enum.IsDefined(pretraga.Status.Value)))
            throw new PoslovnaGreskaException("Izabrani status ili sortiranje nije ispravno.");

        IEnumerable<Igra> rezultat = await igre.DohvatiSveAsync(true, cancellationToken);
        var tekst = pretraga.Tekst?.Trim();
        if (!string.IsNullOrEmpty(tekst))
            rezultat = rezultat.Where(i => i.Naziv.Contains(tekst, StringComparison.OrdinalIgnoreCase)
                || (i.Developer?.Contains(tekst, StringComparison.OrdinalIgnoreCase) ?? false)
                || (i.Izdavac?.Contains(tekst, StringComparison.OrdinalIgnoreCase) ?? false));
        if (pretraga.Status.HasValue)
            rezultat = rezultat.Where(i => i.Status == pretraga.Status);
        if (pretraga.ZanrId.HasValue)
            rezultat = rezultat.Where(i => i.Zanrovi.Any(z => z.Id == pretraga.ZanrId));
        if (pretraga.PlatformaId.HasValue)
            rezultat = rezultat.Where(i => i.Platforme.Any(p => p.Id == pretraga.PlatformaId));
        if (pretraga.SamoOmiljene)
            rezultat = rezultat.Where(i => i.Omiljena);

        // nepoznata godina i nepostavljena ocena ostaju na kraju u oba smera
        IOrderedEnumerable<Igra> sortirano = pretraga.Sortiranje switch
        {
            SortiranjeIgara.GodinaIzdanja => pretraga.Opadajuce
                ? rezultat.OrderBy(i => i.GodinaIzdanja is null).ThenByDescending(i => i.GodinaIzdanja)
                : rezultat.OrderBy(i => i.GodinaIzdanja is null).ThenBy(i => i.GodinaIzdanja),
            SortiranjeIgara.Ocena => pretraga.Opadajuce
                ? rezultat.OrderBy(i => i.Ocena is null).ThenByDescending(i => i.Ocena)
                : rezultat.OrderBy(i => i.Ocena is null).ThenBy(i => i.Ocena),
            SortiranjeIgara.BrojSati => pretraga.Opadajuce
                ? rezultat.OrderByDescending(i => i.BrojSati) : rezultat.OrderBy(i => i.BrojSati),
            SortiranjeIgara.DatumDodavanja => pretraga.Opadajuce
                ? rezultat.OrderByDescending(i => i.DatumDodavanja) : rezultat.OrderBy(i => i.DatumDodavanja),
            _ => pretraga.Opadajuce
                ? rezultat.OrderByDescending(i => i.Naziv, StringComparer.OrdinalIgnoreCase)
                : rezultat.OrderBy(i => i.Naziv, StringComparer.OrdinalIgnoreCase)
        };
        return sortirano.ThenBy(i => i.Id).ToList();
    }

    private readonly IIgraRepository igre;
    private readonly IZanrRepository zanrovi;
    private readonly IPlatformaRepository platforme;

    public IgraService(IIgraRepository igre, IZanrRepository zanrovi, IPlatformaRepository platforme)
    {
        this.igre = igre;
        this.zanrovi = zanrovi;
        this.platforme = platforme;
    }

    public Task<List<Igra>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => igre.DohvatiSveAsync(ukljuciVeze: true, cancellationToken: cancellationToken);

    public Task<Igra?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => igre.DohvatiPoIdAsync(id, ukljuciVeze: true, cancellationToken: cancellationToken);

    public async Task<Igra> DodajAsync(IgraPodaci podaci, CancellationToken cancellationToken = default)
    {
        var igra = new Igra();
        await PrimeniPodatkeAsync(igra, podaci, cancellationToken);
        await igre.DodajAsync(igra, cancellationToken);
        return igra;
    }

    public async Task IzmeniAsync(int id, IgraPodaci podaci, CancellationToken cancellationToken = default)
    {
        var igra = await ZahtevajIgruAsync(id, cancellationToken);
        await PrimeniPodatkeAsync(igra, podaci, cancellationToken);
        await SacuvajIzmenuAsync(igra, cancellationToken);
    }

    public async Task ObrisiAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!await igre.ObrisiAsync(id, cancellationToken))
            throw new PoslovnaGreskaException("Igra ne postoji.");
    }

    public async Task PromeniStatusAsync(int id, StatusIgre status, CancellationToken cancellationToken = default)
    {
        ProveriStatus(status);
        var igra = await ZahtevajIgruAsync(id, cancellationToken);
        igra.Status = status;
        await SacuvajIzmenuAsync(igra, cancellationToken);
    }

    public async Task PromeniOcenuAsync(int id, int? ocena, CancellationToken cancellationToken = default)
    {
        ProveriOcenu(ocena);
        var igra = await ZahtevajIgruAsync(id, cancellationToken);
        igra.Ocena = ocena;
        await SacuvajIzmenuAsync(igra, cancellationToken);
    }

    public async Task PromeniBrojSatiAsync(int id, int brojSati, CancellationToken cancellationToken = default)
    {
        ProveriBrojSati(brojSati);
        var igra = await ZahtevajIgruAsync(id, cancellationToken);
        igra.BrojSati = brojSati;
        await SacuvajIzmenuAsync(igra, cancellationToken);
    }

    public async Task PostaviOmiljenuAsync(int id, bool omiljena, CancellationToken cancellationToken = default)
    {
        var igra = await ZahtevajIgruAsync(id, cancellationToken);
        igra.Omiljena = omiljena;
        await SacuvajIzmenuAsync(igra, cancellationToken);
    }

    private async Task<Igra> ZahtevajIgruAsync(int id, CancellationToken cancellationToken)
        => await DohvatiPoIdAsync(id, cancellationToken)
            ?? throw new PoslovnaGreskaException("Igra ne postoji.");

    private async Task SacuvajIzmenuAsync(Igra igra, CancellationToken cancellationToken)
    {
        if (!await igre.IzmeniAsync(igra, cancellationToken))
            throw new PoslovnaGreskaException("Igra ne postoji.");
    }

    private async Task PrimeniPodatkeAsync(Igra igra, IgraPodaci podaci, CancellationToken cancellationToken)
    {
        if (podaci is null || string.IsNullOrWhiteSpace(podaci.Naziv))
            throw new PoslovnaGreskaException("Naziv igre je obavezan.");
        ProveriOcenu(podaci.Ocena);
        ProveriBrojSati(podaci.BrojSati);
        ProveriStatus(podaci.Status);
        if (podaci.GodinaIzdanja is int godina && (godina < 1950 || godina > DateTime.UtcNow.Year + 5))
            throw new PoslovnaGreskaException("Godina izdanja mora biti od 1950. do pet godina posle tekuce godine.");
        if (podaci.ZanrIds is null || podaci.PlatformaIds is null)
            throw new PoslovnaGreskaException("Kolekcije zanrova i platformi moraju biti prosledjene.");
        if (podaci.ZanrIds.Distinct().Count() != podaci.ZanrIds.Count ||
            podaci.PlatformaIds.Distinct().Count() != podaci.PlatformaIds.Count)
            throw new PoslovnaGreskaException("Zanrovi i platforme ne smeju biti ponovljeni.");

        var izabraniZanrovi = new List<Zanr>();
        foreach (var id in podaci.ZanrIds)
            izabraniZanrovi.Add(await zanrovi.DohvatiPoIdAsync(id, cancellationToken)
                ?? throw new PoslovnaGreskaException($"Zanr sa ID-em {id} ne postoji."));
        var izabranePlatforme = new List<Platforma>();
        foreach (var id in podaci.PlatformaIds)
            izabranePlatforme.Add(await platforme.DohvatiPoIdAsync(id, cancellationToken)
                ?? throw new PoslovnaGreskaException($"Platforma sa ID-em {id} ne postoji."));

        igra.Naziv = podaci.Naziv.Trim();
        igra.Opis = podaci.Opis;
        igra.Beleske = podaci.Beleske;
        igra.GodinaIzdanja = podaci.GodinaIzdanja;
        igra.Developer = podaci.Developer;
        igra.Izdavac = podaci.Izdavac;
        igra.Status = podaci.Status;
        igra.Ocena = podaci.Ocena;
        igra.BrojSati = podaci.BrojSati;
        igra.Omiljena = podaci.Omiljena;
        igra.Zanrovi = izabraniZanrovi;
        igra.Platforme = izabranePlatforme;
    }

    private static void ProveriOcenu(int? ocena)
    {
        if (ocena is < 1 or > 10)
            throw new PoslovnaGreskaException("Ocena mora biti od 1 do 10.");
    }

    private static void ProveriBrojSati(int brojSati)
    {
        if (brojSati < 0)
            throw new PoslovnaGreskaException("Broj sati ne sme biti negativan.");
    }

    private static void ProveriStatus(StatusIgre status)
    {
        if (!Enum.IsDefined(status))
            throw new PoslovnaGreskaException("Status igre nije ispravan.");
    }
}
