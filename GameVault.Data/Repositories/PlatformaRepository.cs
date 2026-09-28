using GameVault.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data.Repositories;

public class PlatformaRepository : IPlatformaRepository
{
    private readonly GameVaultDbContext context;

    public PlatformaRepository(GameVaultDbContext context)
    {
        this.context = context;
    }

    public Task<List<Platforma>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => context.Platforme.AsNoTracking().OrderBy(p => p.Naziv).ThenBy(p => p.Id).ToListAsync(cancellationToken);

    public Task<Platforma?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => context.Platforme.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task DodajAsync(Platforma platforma, CancellationToken cancellationToken = default)
    {
        context.Platforme.Add(platforma);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> PostojiPoNazivuAsync(string naziv, CancellationToken cancellationToken = default)
        => context.Platforme.AnyAsync(p => p.Naziv == naziv, cancellationToken);
}
