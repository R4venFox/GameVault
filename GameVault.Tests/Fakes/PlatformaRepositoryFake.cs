using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Tests.Fakes;

internal class PlatformaRepositoryFake : IPlatformaRepository
{
    public List<Platforma> Zapisi { get; } = new();
    public Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.ToList());
    public Task<Platforma?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.SingleOrDefault(z => z.Id == id));
    public Task DodajAsync(Platforma zapis, CancellationToken cancellationToken = default)
    {
        zapis.Id = Zapisi.Count + 1;
        Zapisi.Add(zapis);
        return Task.CompletedTask;
    }
    public Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.Any(z => z.Naziv == naziv));
}
