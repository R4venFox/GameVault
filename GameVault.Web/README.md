# Pokretanje aplikacije

Iz korena solution-a:

```powershell
dotnet restore GameVault.sln
dotnet tool restore
```

Primeniti migracije iz direktorijuma `GameVault.Web`, kako bi SQLite fajl
bio na istom mestu kao pri pokretanju aplikacije:

```powershell
cd GameVault.Web
dotnet ef database update --project ../GameVault.Data --startup-project .
dotnet run
```

Aplikacija je dostupna na `https://localhost:7180`. Za lokalni HTTPS po potrebi
podesiti poverenje u razvojni sertifikat komandom `dotnet dev-certs https --trust`.

Migracije se ne primenjuju automatski pri pokretanju. Baza se ne brise i nema
automatskog dodavanja podataka. Izbori zanrova i platformi prikazuju zapise koji
vec postoje u bazi. Zanrovima i platformama upravlja se preko linkova u navigaciji.
Zapisi povezani sa igrama ne mogu se obrisati dok se njihove veze ne uklone iz igara.
