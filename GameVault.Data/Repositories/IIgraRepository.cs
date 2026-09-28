using GameVault.Data.Models;

namespace GameVault.Data.Repositories;

public interface IIgraRepository
{
    Task<List<Igra>> DohvatiSveAsync(bool ukljuciVeze = true, CancellationToken cancellationToken = default);
    Task<Igra?> DohvatiPoIdAsync(int id, bool ukljuciVeze = true, CancellationToken cancellationToken = default);
    // zanrovi i platforme moraju imati postojece ID vrednosti
    Task DodajAsync(Igra igra, CancellationToken cancellationToken = default);
    // kolekcije zanrova i platformi predstavljaju kompletan novi skup veza
    Task<bool> IzmeniAsync(Igra igra, CancellationToken cancellationToken = default);
    Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> PostojiAsync(int id, CancellationToken cancellationToken = default);
}
