using GameVault.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data.Repositories;

public class ZanrRepository : IZanrRepository
{
    public async Task<bool> IzmeniAsync(int id, string naziv, CancellationToken cancellationToken = default)
    {
        var zapis = await context.Zanrovi.SingleOrDefaultAsync(z => z.Id == id, cancellationToken);
        if (zapis is null)
            return false;
        zapis.Naziv = naziv;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> KoristiSeAsync(int id, CancellationToken cancellationToken = default)
        => context.Zanrovi.AnyAsync(z => z.Id == id && z.Igre.Any(), cancellationToken);

    public async Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default)
    {
        // uslov u istom upitu sprecava brisanje ako je veza u medjuvremenu dodata
        return await context.Zanrovi.Where(z => z.Id == id && !z.Igre.Any())
            .ExecuteDeleteAsync(cancellationToken) == 1;
    }

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
