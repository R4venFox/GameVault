using GameVault.Data.Models;

namespace GameVault.Business.Services;

public interface IPlatformaService
{
    Task<Platforma?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default);
    Task ObrisiAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Platforma> DodajAsync(string naziv, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
