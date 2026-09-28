## Autor

Ime i prezime: Predrag Davidovic
Broj indeksa: SI 29/22
Uloga: Projektovanje arhitekture, implementacija poslovne logike,
baze podataka, korisnickog interfejsa i testiranje aplikacije.


# GameVault

GameVault je web aplikacija za vođenje lične kolekcije video igara, razvijena kao seminarski projekat iz predmeta **Razvoj softvera otvorenog koda**. Namenjena je evidenciji igara, praćenju napretka i čuvanju ličnih ocena i beleški.

## Funkcionalnosti

- Dodavanje, pregled, izmena i brisanje igara.
- Upravljanje žanrovima i platformama; jedna igra može imati više žanrova i platformi.
- Statusi: planirana, u toku, završena i napuštena.
- Ocena, broj sati, omiljene igre i lične beleške odvojene od opisa igre.
- Pretraga po delu naziva, developeru ili izdavaču, bez razlikovanja velikih i malih slova.
- Kombinovanje filtera po statusu, žanru, platformi i omiljenim igrama.
- Sortiranje po nazivu, godini izdanja, oceni, broju sati i datumu dodavanja, u oba smera.
- Statistika cele kolekcije: broj igara po statusu, omiljene igre, ukupan broj sati, prosečna ocena i najzastupljeniji žanr i platforma.

## Tehnologije

C#, .NET 8, ASP.NET Core MVC, Entity Framework Core 8, SQLite, Razor Views, Bootstrap 5.3.3 i xUnit. Verzije direktnih paketa i njihove licence navedene su u [pregledu licenci](THIRD_PARTY_LICENSES.md).

## Arhitektura

Projekat koristi jednostavnu troslojnu arhitekturu:

```text
GameVault.Web
      |
      v
GameVault.Business
      |
      v
GameVault.Data
      |
      v
SQLite
```

Osnovni tok obrade zahteva je **Controller → Service → Repository → Database**.

| Projekat | Odgovornost |
| --- | --- |
| `GameVault.Web` | MVC kontroleri, ViewModel klase, Razor prikazi i Bootstrap interfejs. Kontroleri pozivaju Business servise. |
| `GameVault.Business` | Servisi, poslovna pravila, pretraga i izračunavanje statistike. Podacima pristupa preko repository interfejsa. |
| `GameVault.Data` | Domenski modeli, repository interfejsi i implementacije, DbContext, EF konfiguracija i migracije. |
| `GameVault.Tests` | Testovi modela, baze, repository-ja, servisa, kontrolera i DI registracija. Referencira Business i Web projekat. |

Struktura solution-a:

```text
GameVault.sln
├── GameVault.Web/          Controllers, Models, Views, wwwroot
├── GameVault.Business/     Models, Services
├── GameVault.Data/         Models, Repositories, Migrations
├── GameVault.Tests/        Testovi i zamene za zavisnosti
├── .config/dotnet-tools.json
├── README.md
├── LICENSE
└── THIRD_PARTY_LICENSES.md
```

Modeli `Igra`, `Zanr` i `Platforma` nalaze se u Data projektu. Many-to-many veze koriste spojne tabele bez posebnih spojnih klasa. Servisi i repository implementacije registrovani su kroz dependency injection sa `Scoped` životnim vekom.

## Zahtevi

- .NET 8 SDK ili kompatibilan noviji SDK uz instalirane .NET 8 i ASP.NET Core 8 runtime komponente.
- Pristup NuGet izvoru pri prvom preuzimanju paketa i lokalnog alata `dotnet-ef`.
- Web pregledač i dozvola upisa u direktorijum aplikacije radi SQLite baze.

Nije potrebna posebna instalacija servera baze, Node.js-a ili JavaScript framework-a. Bootstrap CSS se nalazi u repozitorijumu.

## Pokretanje

Komande izvršavati iz direktorijuma koji sadrži `GameVault.sln`:

```sh
dotnet restore GameVault.sln
dotnet tool restore
dotnet build GameVault.sln --no-restore
```

Zatim preći u Web projekat, primeniti migracije i pokrenuti aplikaciju:

```sh
cd GameVault.Web
dotnet ef database update --project ../GameVault.Data --startup-project .
dotnet run
```

Početna stranica je na **https://localhost:7180**. HTTP adresa `http://localhost:5180` preusmerava na HTTPS. Za lokalni razvoj, ako pregledač ne veruje razvojnom sertifikatu, proveriti podešavanje poverenja:

```sh
dotnet dev-certs https --trust
```

Postupak poverenja u sertifikat može zavisiti od operativnog sistema i pregledača.

Pre ponovnog build-a zaustaviti pokrenutu aplikaciju sa Ctrl+C, kako proces ne bi držao zaključane izlazne fajlove.

Na praznoj bazi najpre se mogu dodati žanrovi i platforme preko navigacije, a zatim igre. Žanrovi i platforme nisu obavezni za unos igre. Za izbor više stavki u formi koristi se Ctrl, odnosno Command na macOS-u.

## SQLite i migracije

Konekcioni string `ConnectionStrings:GameVault` nalazi se u `GameVault.Web/appsettings.json` i podrazumevano je `Data Source=gamevault.db`. Uz navedene komande baza se nalazi u direktorijumu `GameVault.Web`.

Migracije se primenjuju eksplicitno, ne automatski pri pokretanju. Komanda `database update` kreira bazu ako ne postoji i primenjuje migracije koje još nisu izvršene. Ne resetuje kolekciju i ne dodaje probne podatke.

- `PocetnaBaza` postavlja tabele, veze i ograničenja.
- `DodateBeleske` dodaje opciono polje za lične beleške.

Pri novom preuzimanju projekta treba primeniti postojeće migracije, a ne praviti novu početnu migraciju. SQLite fajlovi i pomoćni fajlovi baze nisu deo Git repozitorijuma; migracije jesu.

## Testovi

Iz korena solution-a:

```sh
dotnet test GameVault.sln
```

Ako su restore i build već izvršeni:

```sh
dotnet test GameVault.sln --no-build --no-restore
```

Testovi obuhvataju validaciju, čuvanje podataka, veze, migracije, bezbedno brisanje, servise, MVC akcije, pretragu, filtere, sortiranje i statistiku. Testovi baze koriste izolovani SQLite u memoriji, a unit testovi servisa koriste zamene za repository-je. Ne menjaju lokalnu korisničku bazu. Pri završnoj proveri prošao je 151 test, a build je završen bez grešaka i upozorenja.

## Validacija

Naziv igre je obavezan i ne može sadržati samo praznine. Ocena je opciona i mora biti od 1 do 10; broj sati ne sme biti negativan. Godina izdanja, ako je uneta, mora biti između 1950. i tekuće godine + 5. Prosleđeni žanrovi i platforme moraju postojati i ne smeju se ponavljati unutar igre.

Nazivi žanrova i platformi proveravaju se bez razlikovanja veličine slova i uz uklanjanje okolnih razmaka. Brisanje zapisa povezanog sa igrama se odbija uz poruku korisniku. Forme koriste osnovnu MVC validaciju, a Business sloj proverava poslovna pravila. POST akcije imaju antiforgery zaštitu.

## Poznata ograničenja

- Aplikacija je namenjena lokalnom, jednokorisničkom radu i nema sistem korisničkih naloga.
- Pretraga, filtriranje i sortiranje rade nad kolekcijom učitanom u memoriju. To odgovara maloj ličnoj kolekciji, ali nije predviđeno za veoma velike skupove podataka. Statistika se takođe računa u memoriji.
- Provera duplikata naziva žanrova i platformi nije projektovana za ozbiljne konkurentne upise; nije garantovana zabrana različito napisanih naziva pri istovremenim zahtevima.
- Statistika prikazuje žanr i platformu zastupljene u najviše igara. Prosek obuhvata samo ocenjene igre. Prazna kolekcija prikazuje nule i poruke o nedostajućim podacima.

## Licenca

GameVault je objavljen pod [MIT licencom](LICENSE). Autor naveden u licenci je **R4venFox**.

Biblioteke zadržavaju svoje licence. Njihove verzije, namene, izvori i obaveštenja navedeni su u [THIRD_PARTY_LICENSES.md](THIRD_PARTY_LICENSES.md).
