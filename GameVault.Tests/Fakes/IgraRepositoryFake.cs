using GameVault.Data.Models;
using GameVault.Data.Repositories;

namespace GameVault.Tests.Fakes;

internal class IgraRepositoryFake : IIgraRepository
{
    public List<Igra> Zapisi { get; } = new();
    public int BrojIzmena { get; private set; }
    public bool? UkljuceneVeze { get; private set; }
    public bool OdbijIzmenu { get; set; }

    public Task<List<Igra>> DohvatiSveAsync(bool ukljuciVeze = true, CancellationToken cancellationToken = default)
    {
        UkljuceneVeze = ukljuciVeze;
        return Task.FromResult(Zapisi.ToList());
    }

    public Task<Igra?> DohvatiPoIdAsync(int id, bool ukljuciVeze = true, CancellationToken cancellationToken = default)
    {
        UkljuceneVeze = ukljuciVeze;
        return Task.FromResult(Zapisi.SingleOrDefault(i => i.Id == id));
    }

    public Task DodajAsync(Igra igra, CancellationToken cancellationToken = default)
    {
        igra.Id = Zapisi.Count + 1;
        Zapisi.Add(igra);
        return Task.CompletedTask;
    }

    public Task<bool> IzmeniAsync(Igra igra, CancellationToken cancellationToken = default)
    {
        var index = Zapisi.FindIndex(i => i.Id == igra.Id);
        if (index < 0 || OdbijIzmenu)
            return Task.FromResult(false);
        Zapisi[index] = igra;
        BrojIzmena++;
        return Task.FromResult(true);
    }

    public Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.RemoveAll(i => i.Id == id) > 0);

    public Task<bool> PostojiAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Zapisi.Any(i => i.Id == id));
}
