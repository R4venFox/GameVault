using GameVault.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data.Repositories;

public class PlatformaRepository : IPlatformaRepository
{
    public async Task<bool> IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default)
    {
        var zapis = await context.Platforme.SingleOrDefaultAsync(z => z.Id == id, cancellationToken);
        if (zapis is null)
            return false;
        zapis.Naziv = naziv;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> KoristiSeAsync(int id, CancellationToken cancellationToken = default)
        => context.Platforme.AnyAsync(z => z.Id == id && z.Igre.Any(), cancellationToken);

    public async Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default)
    {
        // uslov u istom upitu sprecava brisanje ako je veza u medjuvremenu dodata
        return await context.Platforme.Where(z => z.Id == id && !z.Igre.Any())
            .ExecuteDeleteAsync(cancellationToken) == 1;
    }

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
