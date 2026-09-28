using GameVault.Data.Models;

namespace GameVault.Data.Repositories;

public interface IPlatformaRepository
{
    Task<bool> IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default);
    Task<bool> KoristiSeAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Platforma?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task DodajAsync(Platforma platforma, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
