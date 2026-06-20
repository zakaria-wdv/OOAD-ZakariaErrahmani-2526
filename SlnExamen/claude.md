# Claude Agent Instructions — WPF Dierenartspraktijk

## Doel
WPF-toepassing voor een dierenartspraktijk (examenproject OOAD 25-26, Odisee).
Eigenaars registreren dieren als patiënt. De arts kan dieren raadplegen, filteren,
nieuwe dieren registreren en dieren opnemen.

## Architectuur
- **WPFDierenarts** (.NET 10, WPF): enkel UI-logica
- **CLDierenarts** (.NET 10, Class Library): alle domeinlogica én alle datatoegang
- **SQLite** via NuGet `Microsoft.Data.Sqlite` — databasebestand `dieren.db` naast de .exe

### Kritieke architectuurregel
Alle datatoegang (inlezen, toevoegen, wijzigen) zit IN de klassen van de class library zelf.
GEEN aparte datalayer / datacontext / datahelper / repository.
De code-behind mag op GEEN ENKELE manier weten waar de data vandaan komt (csv/json/sql).
Alle communicatie met data verloopt via de class library.

## Klassen (CLDierenarts)
- `Urgentie` — enum: Laag, Normaal, Spoed
- `Eigenaar` — Id, Voornaam, Achternaam; static LaadAlle()
- `Dier` (abstract) — Id, Naam, Eigenaar, Geboortedatum, Gewicht, Urgentie, IsOpgenomen,
  DatumOpgenomen; abstract GeefInfo(); override ToString(); static DatabasePad, InitialiseerDatabase(),
  LaadAlle(), LaadGefilterd(), VoegToe(), Opnemen()
- `Hond : Dier` — extra: Ras; override GeefInfo()
- `Kat : Dier` — extra: IsGevaccineerd; override GeefInfo()
- `DierValidator` — MinAantalTekensRas (readonly=3), IsGeldigeNaam(), IsGeldigRas()

## OO-concepten (verplicht correct toepassen)
- `static` — enkel voor DatabasePad en data-methodes
- `abstract` — klasse Dier, methode GeefInfo()
- `virtual`/`override` — ToString() in Dier; GeefInfo() in Hond en Kat
- `:base()` — Hond/Kat volledige constructor → Dier
- `:this()` — Hond/Kat korte constructor → eigen volledige constructor
- Readonly properties — `{ get; }` overal waar waarde na constructie niet wijzigt
- Compositie — Dier heeft een Eigenaar-object
- Pure methodes — IsGeldigeNaam, IsGeldigRas, GeefInfo, ToString

## ABSOLUUT VERBODEN (geeft 0/20)
- `var` — altijd expliciete types
- LINQ
- DataGrid / GridView / ListView
- Databinding
- `async` / `await`
- Tuples
- `MessageBox` voor foutmeldingen — gebruik TextBlock
- `break` in lussen
- `dynamic`
- Case guards
- Anonieme objecten
- User controls
- `out` parameters
- `Invoke`
- Structs
- Type switches

## Cursusconventies
- camelCase voor lokale variabelen, PascalCase voor properties/methodes
- Private hulpvariabelen met `_` prefix
- Automatische properties als er geen validatie is
- `decimal` voor geldbedragen (niet van toepassing hier)
- Geen Console of WPF-controls in de class library
- `try-catch` enkel rond SQLite-aanroepen, niet rond eigen logica
- Foutmeldingen via TextBlock, niet MessageBox
- ListBox vullen via `Items.Add(new ListBoxItem { Content = ..., Tag = ... })`
- Conversie via `ToString()` en `Convert.To...()`
