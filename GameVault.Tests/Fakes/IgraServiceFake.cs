using GameVault.Business;
using GameVault.Business.Models;
using GameVault.Business.Services;
using GameVault.Data.Models;

namespace GameVault.Tests.Fakes;

internal class IgraServiceFake : IIgraService
{
    public IgraPretraga? PoslednjaPretraga { get; private set; }
    public Task<List<Igra>> PretraziAsync(IgraPretraga pretraga, CancellationToken cancellationToken = default)
    {
        ProveriGresku();
        PoslednjaPretraga = pretraga;
        return DohvatiSveAsync(cancellationToken);
    }
    public Igra? Igra { get; set; } = new() { Id = 1, Naziv = "Portal" };
    public string? Greska { get; set; }
    public IgraPodaci? SacuvaniPodaci { get; private set; }
    public int? IzmenjenId { get; private set; }
    public int? ObrisanId { get; private set; }

    public Task<List<Igra>> DohvatiSveAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Igra is null ? new List<Igra>() : new List<Igra> { Igra });

    public Task<Igra?> DohvatiPoIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Igra?.Id == id ? Igra : null);

    public Task<Igra> DodajAsync(IgraPodaci podaci, CancellationToken cancellationToken = default)
    {
        ProveriGresku();
        SacuvaniPodaci = podaci;
        return Task.FromResult(new Igra { Id = 2, Naziv = podaci.Naziv });
    }

    public Task IzmeniAsync(int id, IgraPodaci podaci, CancellationToken cancellationToken = default)
    {
        ProveriGresku();
        SacuvaniPodaci = podaci;
        IzmenjenId = id;
        return Task.CompletedTask;
    }

    public Task ObrisiAsync(int id, CancellationToken cancellationToken = default)
    {
        ProveriGresku();
        ObrisanId = id;
        return Task.CompletedTask;
    }

    private void ProveriGresku()
    {
        if (Greska is not null)
            throw new PoslovnaGreskaException(Greska);
    }

    public Task PromeniStatusAsync(int id, StatusIgre status, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
    public Task PromeniOcenuAsync(int id, int? ocena, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
    public Task PromeniBrojSatiAsync(int id, int brojSati, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
    public Task PostaviOmiljenuAsync(int id, bool omiljena, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
