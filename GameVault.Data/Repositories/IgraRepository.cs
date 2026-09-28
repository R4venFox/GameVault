using GameVault.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data.Repositories;

public class IgraRepository : IIgraRepository
{
    private readonly GameVaultDbContext context;

    public IgraRepository(GameVaultDbContext context)
    {
        this.context = context;
    }

    public Task<List<Igra>> DohvatiSveAsync(bool ukljuciVeze = true, CancellationToken cancellationToken = default)
        => Upit(ukljuciVeze).OrderBy(i => i.Naziv).ThenBy(i => i.Id).ToListAsync(cancellationToken);

    public Task<Igra?> DohvatiPoIdAsync(int id, bool ukljuciVeze = true, CancellationToken cancellationToken = default)
        => Upit(ukljuciVeze).SingleOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task DodajAsync(Igra igra, CancellationToken cancellationToken = default)
    {
        var (zanrovi, platforme) = await DohvatiVezeAsync(igra, cancellationToken);
        igra.Zanrovi = zanrovi;
        igra.Platforme = platforme;
        context.Igre.Add(igra);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IzmeniAsync(Igra igra, CancellationToken cancellationToken = default)
    {
        var postojeca = await context.Igre.Include(i => i.Zanrovi).Include(i => i.Platforme)
            .SingleOrDefaultAsync(i => i.Id == igra.Id, cancellationToken);
        if (postojeca is null)
            return false;

        var (zanrovi, platforme) = await DohvatiVezeAsync(igra, cancellationToken);
        context.Entry(postojeca).CurrentValues.SetValues(igra);
        postojeca.Zanrovi.Clear();
        foreach (var zanr in zanrovi)
            postojeca.Zanrovi.Add(zanr);
        postojeca.Platforme.Clear();
        foreach (var platforma in platforme)
            postojeca.Platforme.Add(platforma);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ObrisiAsync(int id, CancellationToken cancellationToken = default)
    {
        var igra = await context.Igre.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (igra is null)
            return false;

        context.Igre.Remove(igra);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> PostojiAsync(int id, CancellationToken cancellationToken = default)
        => context.Igre.AnyAsync(i => i.Id == id, cancellationToken);

    private IQueryable<Igra> Upit(bool ukljuciVeze)
    {
        var upit = context.Igre.AsNoTracking();
        return ukljuciVeze ? upit.Include(i => i.Zanrovi).Include(i => i.Platforme) : upit;
    }

    private async Task<(List<Zanr>, List<Platforma>)> DohvatiVezeAsync(
        Igra igra, CancellationToken cancellationToken)
    {
        // veze koriste postojece zapise, bez ponovnog dodavanja ili izmene njihovih naziva
        var zanrIds = igra.Zanrovi.Select(z => z.Id).Distinct().ToList();
        var platformaIds = igra.Platforme.Select(p => p.Id).Distinct().ToList();
        var zanrovi = await context.Zanrovi.Where(z => zanrIds.Contains(z.Id)).ToListAsync(cancellationToken);
        var platforme = await context.Platforme.Where(p => platformaIds.Contains(p.Id)).ToListAsync(cancellationToken);

        if (zanrovi.Count != zanrIds.Count || platforme.Count != platformaIds.Count)
            throw new ArgumentException("Neki od navedenih zanrova ili platformi ne postoje.", nameof(igra));

        return (zanrovi, platforme);
    }
}
