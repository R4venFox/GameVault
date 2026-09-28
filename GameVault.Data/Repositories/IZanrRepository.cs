using GameVault.Data.Models;

namespace GameVault.Data.Repositories;

public interface IZanrRepository
{
    Task<bool> IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default);
    Task<bool> KoristiSeAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Zanr>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Zanr?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task DodajAsync(Zanr zanr, CancellationToken cancellationToken = default);
    Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default);
}
