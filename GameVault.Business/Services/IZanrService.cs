using GameVault.Data.Models;

namespace GameVault.Business.Services;

public interface IZanrService
{
    Task<List<Zanr>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Zanr> DodajAsync(string naziv, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
