using GameVault.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data.Repositories;

public class ZanrRepository : IZanrRepository
{
    private readonly GameVaultDbContext context;

    public ZanrRepository(GameVaultDbContext context)
    {
        this.context = context;
    }

    public Task<List<Zanr>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => context.Zanrovi.AsNoTracking().OrderBy(z => z.Naziv).ThenBy(z => z.Id).ToListAsync(cancellationToken);

    public Task<Zanr?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => context.Zanrovi.AsNoTracking().SingleOrDefaultAsync(z => z.Id == id, cancellationToken);

    public async Task DodajAsync(Zanr zanr, CancellationToken cancellationToken = default)
    {
        context.Zanrovi.Add(zanr);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default)
        => context.Zanrovi.AnyAsync(z => z.Naziv == naziv, cancellationToken);
}
