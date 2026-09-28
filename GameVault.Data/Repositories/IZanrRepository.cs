using GameVault.Data.Models;

namespace GameVault.Data.Repositories;

public interface IZanrRepository
{
    Task<List<Zanr>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Zanr?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task DodajAsync(Zanr zanr, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
