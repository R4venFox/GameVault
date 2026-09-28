using GameVault.Data.Models;

namespace GameVault.Business.Services;

public interface IPlatformaService
{
    Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Platforma> DodajAsync(string naziv, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
