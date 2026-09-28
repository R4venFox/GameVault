using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Tests.Fakes;

internal class ZanrRepositoryFake : IZanrRepository
{
    public HashSet<int> KorisceniId { get; } = new();
    public Task<bool> KoristiSeAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(KorisceniId.Contains(id));
    public Task<bool> IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default)
    {
        var zapis = Zapisi.SingleOrDefault(z => z.Id == id);
        if (zapis is null)
            return Task.FromResult(false);
        zapis.Naziv = naziv;
        return Task.FromResult(true);
    }
    public Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(!KorisceniId.Contains(id) && Zapisi.RemoveAll(z => z.Id == id) == 1);

    public List<Zanr> Zapisi { get; } = new();
    public Task<List<Zanr>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.ToList());
    public Task<Zanr?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.SingleOrDefault(z => z.Id == id));
    public Task DodajAsync(Zanr zapis, CancellationToken cancellationToken = default)
    {
        zapis.Id = Zapisi.Count + 1;
        Zapisi.Add(zapis);
        return Task.CompletedTask;
    }
    public Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.Any(z => z.Naziv == naziv));
}
