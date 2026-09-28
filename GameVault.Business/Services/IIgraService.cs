using GameVault.Business.Models;
using GameVault.Data.Models;

namespace GameVault.Business.Services;

public interface IIgraService
{
    Task<List<Igra>> PretraziAsync(IgraPretraga pretraga, CancellationToken cancellationToken = default);
    Task<List<Igra>> DohvatiSveAsync(CancellationToken cancellationToken = default);
    Task<Igra?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Igra> DodajAsync(IgraPodaci podaci, CancellationToken cancellationToken = default);
    Task IzmeniAsync(int id, IgraPodaci podaci, CancellationToken cancellationToken = default);
    Task ObrisiAsync(int id, CancellationToken cancellationToken = default);
    Task PromeniStatusAsync(int id, StatusIgre status, CancellationToken cancellationToken = default);
    Task PromeniOcenuAsync(int id, int? ocena, CancellationToken cancellationToken = default);
    Task PromeniBrojSatiAsync(int id, int brojSati, CancellationToken cancellationToken = default);
    Task PostaviOmiljenuAsync(int id, bool omiljena, CancellationToken cancellationToken = default);
}
