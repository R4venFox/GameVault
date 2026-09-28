using GameVault.Data.Models;

namespace GameVault.Business.Services;

public interface IZanrService
{
    Task<Zanr?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default);
    Task ObrisiAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Zanr>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Zanr> DodajAsync(string naziv, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
