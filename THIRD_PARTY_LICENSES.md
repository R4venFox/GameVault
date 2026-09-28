# Licence zavisnosti

Pregled obuhvata sve direktne `PackageReference` zavisnosti iz četiri projekta, lokalni alat iz `.config/dotnet-tools.json` i lokalni Bootstrap. Verzije i deklarisane licence NuGet paketa proverene su u `.nuspec` metapodacima preuzetih paketa; izvori u tabelama vode do odgovarajuće verzije paketa ili licencnog fajla zvaničnog projekta. Datum provere: 28. septembar 2026.

## Direktni NuGet paketi

| Paket | Verzija | Svrha i projekat | Licenca | Izvor |
| --- | --- | --- | --- | --- |
| Microsoft.Extensions.Configuration.Abstractions | 8.0.0 | Pristup konfiguraciji kroz interfejse; Business | MIT | [Paket](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions/8.0.0), [licenca izvornog commita](https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/LICENSE.TXT) |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.2 | DI ugovori i registracija servisa; Business | MIT | [Paket](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/8.0.2), [licenca izvornog commita](https://github.com/dotnet/runtime/blob/81cabf2857a01351e5ab578947c7403a5b128ad1/LICENSE.TXT) |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.20 | EF Core provider za SQLite; Data | MIT | [Paket](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/8.0.20), [EF Core licenca](https://github.com/dotnet/efcore/blob/a947fe22902f3f0b921f5dafed9f059eaa4d18c6/LICENSE.txt) |
| Microsoft.EntityFrameworkCore.Design | 8.0.20 | Podrška EF alatima i migracijama; Web kao startup projekat | MIT | [Paket](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/8.0.20), [EF Core licenca](https://github.com/dotnet/efcore/blob/a947fe22902f3f0b921f5dafed9f059eaa4d18c6/LICENSE.txt) |
| Microsoft.NET.Test.Sdk | 17.14.1 | Test host i integracija sa `dotnet test`; Tests | MIT | [Paket](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/17.14.1), [VSTest licenca](https://github.com/microsoft/vstest/blob/490850ae3fdc1b470e3804ceab4f6a41cf89ae51/LICENSE) |
| Microsoft.Extensions.Configuration | 8.0.0 | Kreiranje konfiguracije u testovima; Tests | MIT | [Paket](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/8.0.0), [licenca izvornog commita](https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/LICENSE.TXT) |
| xunit | 2.9.3 | Framework za testiranje; Tests | Apache-2.0 | [Paket](https://www.nuget.org/packages/xunit/2.9.3), [licenca izvornog commita](https://github.com/xunit/xunit/blob/9712244020d385955d33136b3fe3e87de43539cd/license.txt) |
| xunit.runner.visualstudio | 3.1.4 | Otkrivanje i izvršavanje xUnit testova kroz VSTest; Tests | Apache-2.0 | [Paket](https://www.nuget.org/packages/xunit.runner.visualstudio/3.1.4), [licenca izvornog commita](https://github.com/xunit/visualstudio.xunit/blob/50e68bbb8b9ddd4b1bbb95d062b62010caf99909/License.txt) |

## Alat i lokalna biblioteka

| Naziv | Verzija | Svrha | Licenca | Izvor |
| --- | --- | --- | --- | --- |
| dotnet-ef | 8.0.20 | Lokalni CLI alat za EF Core migracije | MIT | [Paket](https://www.nuget.org/packages/dotnet-ef/8.0.20), [EF Core licenca](https://github.com/dotnet/efcore/blob/a947fe22902f3f0b921f5dafed9f059eaa4d18c6/LICENSE.txt) |
| Bootstrap | 5.3.3 | CSS za izgled Razor prikaza, u `GameVault.Web/wwwroot/lib/bootstrap` | MIT | [Zvanična licenca za v5.3.3](https://github.com/twbs/bootstrap/blob/v5.3.3/LICENSE), [lokalna licenca](GameVault.Web/wwwroot/lib/bootstrap/LICENSE) |

Verzija Bootstrapa potvrđena je u zaglavlju `bootstrap.min.css`. Lokalni licencni fajl je identičan originalnom, neizmenjenom LICENSE fajlu iz zvaničnog repozitorijuma za verziju 5.3.3. CSS nije menjan.

## Framework i posredne zavisnosti

ASP.NET Core MVC i Razor dolaze kroz `Microsoft.NET.Sdk.Web` i deljeni framework `Microsoft.AspNetCore.App`, a nisu zasebni direktni NuGet paketi ovog projekta. Projekti ciljaju `net8.0`; patch verzija runtime-a zavisi od instalacije. U okruženju provere korišćen je runtime 8.0.21. Osnovni .NET runtime i ASP.NET Core imaju MIT licence: [.NET 8.0.21](https://github.com/dotnet/runtime/blob/v8.0.21/LICENSE.TXT) i [ASP.NET Core 8.0.21](https://github.com/dotnet/aspnetcore/blob/v8.0.21/LICENSE.txt).

SQLite native biblioteka i SQLitePCLRaw dolaze kao posredne zavisnosti EF SQLite paketa. MIT oznaka providera ne predstavlja automatski licencu svih njegovih zavisnosti. Ovaj dokument je pregled direktnih zavisnosti, ne potpuni inventar svih posrednih paketa i runtime komponenti. Njihov konkretan spisak za obnovljeni projekat može se dobiti komandom:

```sh
dotnet list GameVault.sln package --include-transitive
```

## Obaveštenja i redistribucija

- MIT licenca projekta odnosi se na GameVault kod, ne zamenjuje licence biblioteka. Pri redistribuciji njihovih kopija treba zadržati pripadajuće licence i obaveštenja o autorskim pravima.
- Lokalni Bootstrap već sadrži pun MIT tekst i obaveštenje u CSS zaglavlju. Ti fajlovi ostaju u repozitorijumu.
- Zvanični licencni fajlovi xUnit projekata navode Apache-2.0 kao osnovnu licencu, kao i MIT obaveštenja za preuzete delove .NET koda. Njihove neizmenjene kopije sa navedenih commit-a sačuvane su u [xunit-2.9.3-NOTICES.txt](licenses/xunit-2.9.3-NOTICES.txt) i [xunit.runner.visualstudio-3.1.4-NOTICES.txt](licenses/xunit.runner.visualstudio-3.1.4-NOTICES.txt). Pun tekst [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0) treba priložiti ako se distribuiraju njome obuhvaćene biblioteke, a ne samo GameVault izvorni kod.
- Tri direktna `Microsoft.Extensions.*` paketa sadrže `LICENSE.TXT` i `THIRD-PARTY-NOTICES.TXT`. Potonji dokumenti obuhvataju šira obaveštenja .NET runtime-a i ne treba ih izostavljati pri redistribuciji relevantnih komponenti. Izvori su [runtime commit za 8.0.0 pakete](https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/THIRD-PARTY-NOTICES.TXT) i [commit za DI 8.0.2](https://github.com/dotnet/runtime/blob/81cabf2857a01351e5ab578947c7403a5b128ad1/THIRD-PARTY-NOTICES.TXT). Paketi se preuzimaju preko NuGet-a; njihove binarne kopije nisu uključene u ovaj repozitorijum.

## Kompatibilnost

Za pregledane direktne zavisnosti nije utvrđen konflikt sa MIT licencom GameVault izvornog koda. MIT i Apache-2.0 biblioteke zadržavaju svoje uslove; njihovo korišćenje ne znači da se njihove licence menjaju u MIT. xUnit paketi služe test projektu.

Pri pripremi samostalne binarne distribucije treba proveriti i posredne pakete i komponente runtime-a koje se zaista isporučuju, uključujući njihove licence i obaveštenja. Taj širi pregled nije obuhvaćen ovim spiskom direktnih zavisnosti.
