using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Tests.Fakes;

internal class ZanrRepositoryFake : IZanrRepository
{
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
