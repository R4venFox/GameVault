using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Business.Services;

public class PlatformaService : IPlatformaService
{
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
