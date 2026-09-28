using GameVault.Business.Models;

namespace GameVault.Business.Services;

public interface IStatistikaService
{
    Task<StatistikaPodaci> DohvatiAsync(CancellationToken cancellationToken = default);
}
