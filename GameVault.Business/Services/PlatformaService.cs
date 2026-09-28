using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Business.Services;

public class PlatformaService : IPlatformaService
{
    public Task<Platforma?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => repository.DohvatiPoIdAsync(id, cancellationToken);

    public async Task IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default)
    {
        if (await repository.DohvatiPoIdAsync(id, cancellationToken) is null)
            throw new PoslovnaGreskaException("Zapis ne postoji.");
        if (string.IsNullOrWhiteSpace(naziv))
            throw new PoslovnaGreskaException("Naziv je obavezan.");
        naziv = naziv.Trim();
        var zapisi = await repository.DohvatiSveAsync(cancellationToken);
        if (zapisi.Any(z => z.Id != id && string.Equals(z.Naziv.Trim(), naziv, StringComparison.OrdinalIgnoreCase)))
            throw new PoslovnaGreskaException("Zapis sa ovim nazivom vec postoji.");
        if (!await repository.IzmeniAsync(id, naziv, cancellationToken))
            throw new PoslovnaGreskaException("Zapis ne postoji.");
    }

    public async Task ObrisiAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await repository.DohvatiPoIdAsync(id, cancellationToken) is null)
            throw new PoslovnaGreskaException("Zapis ne postoji.");
        if (await repository.KoristiSeAsync(id, cancellationToken))
            throw new PoslovnaGreskaException("Brisanje nije dozvoljeno jer je zapis povezan sa igrama.");
        if (!await repository.ObrisiAsync(id, cancellationToken))
            throw new PoslovnaGreskaException("Zapis nije obrisan jer vise ne postoji ili je povezan sa igrama.");
    }

    private readonly IPlatformaRepository repository;

    public PlatformaService(IPlatformaRepository repository)
    {
        this.repository = repository;
    }

    public Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => repository.DohvatiSveAsync(cancellationToken);

    public async Task<Platforma> DodajAsync(string naziv, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(naziv))
            throw new PoslovnaGreskaException("Naziv je obavezan.");
        naziv = naziv.Trim();
        if (await PostojiPoNazivuAsync(naziv, cancellationToken))
            throw new PoslovnaGreskaException("Zapis sa ovim nazivom vec postoji.");
        var zapis = new Platforma { Naziv = naziv };
        await repository.DodajAsync(zapis, cancellationToken);
        return zapis;
    }

    public async Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(naziv))
            return false;
        var zapisi = await repository.DohvatiSveAsync(cancellationToken);
        return zapisi.Any(z => string.Equals(z.Naziv.Trim(), naziv.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
