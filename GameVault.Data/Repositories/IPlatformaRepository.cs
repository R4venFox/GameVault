using GameVault.Data.Models;

namespace GameVault.Data.Repositories;

public interface IPlatformaRepository
{
    Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Platforma?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task DodajAsync(Platforma platforma, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
