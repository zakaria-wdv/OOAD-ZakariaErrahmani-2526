# Documentatie — WPF Dierenartspraktijk OOAD 25-26

---

## 1. Projectinstructies (inhoud claude.md)

### Doel
WPF-toepassing voor een dierenartspraktijk (examenproject OOAD 25-26, Odisee).
Eigenaars registreren dieren als patiënt. De arts kan dieren raadplegen, filteren,
nieuwe dieren registreren en dieren opnemen.

### Architectuur
- **WPFDierenarts** (.NET 10, WPF): enkel UI-logica
- **CLDierenarts** (.NET 10, Class Library): alle domeinlogica én alle datatoegang
- **SQLite** via NuGet `Microsoft.Data.Sqlite`

**Kritieke architectuurregel**: Alle datatoegang zit IN de klassen van de class library.
GEEN aparte datalayer / datacontext / datahelper / repository.
De code-behind weet niet waar de data vandaan komt.

### Verboden technieken (geeft 0/20)
`var`, LINQ, DataGrid/ListView, databinding, async/await, tuples, MessageBox (gebruik TextBlock),
`break` in lussen, `dynamic`, case guards, anonieme objecten, user controls, `out` parameters,
`Invoke`, structs, type switches.

---

## 2. Initiële prompt van de student

```
Je gaat me helpen een examenproject bouwen voor het vak Application Development (C#, .NET, WPF).
Werk in PLAN MODE: stel eerst een volledig stappenplan op en wacht op mijn goedkeuring
voordat je code schrijft. Daarna werken we FEATURE PER FEATURE in kleine stapjes;
na elke stap stop je zodat ik de wijzigingen kan controleren.

=== DOEL ===
Een WPF-toepassing voor een dierenartspraktijk. Eigenaars laten hun dier registreren als
patiënt. De arts kan dieren raadplegen, filteren, nieuwe dieren registreren en dieren opnemen.

=== ARCHITECTUUR (verplicht) ===
- 1 WPF App (.NET10): WPFDierenarts
- 1 Class Library (.NET10): CLDierenarts
- Databank: SQLite via NuGet package Microsoft.Data.Sqlite (geen aparte server).
- Er is een SQL-bestand en/of CSV aangeleverd; ik geef je de structuur. Maak op basis
  daarvan de SQLite-databank aan en vul ze.
- KRITIEK: alle datatoegang (inlezen, toevoegen, wijzigen) zit IN de klassen van de
  class library zelf. GEEN aparte datalayer / datacontext / datahelper / repository.
  De code-behind mag op GEEN ENKELE manier weten waar de data vandaan komt (csv/json/sql).
  Alle communicatie met data verloopt via de class library.

=== KLASSEN (Class Library CLDierenarts) ===
- Basisklasse Dier (gebruikelijke properties, constructors). Minstens:
    - GeefInfo(): voor de details-weergave
    - ToString(): voor de ListBox; als het dier opgenomen is, moet dat zichtbaar zijn
- Afgeleide klassen Kat en Hond (overerving van Dier):
    - Hond heeft extra eigenschap Ras
    - Kat heeft extra eigenschap of het gevaccineerd is
    - Beide overschrijven GeefInfo() om type-specifieke info te tonen
- Klasse DierValidator (te gebruiken vóór een nieuw dier wordt toegevoegd):
    - readonly property voor het minimaal aantal tekens van het ras
    - IsGeldigeNaam(): naam bestaat enkel uit letters, spaties en koppelteken
    - IsGeldigRas(): ras moet minstens uit 3 tekens bestaan
- Bepaal zelf en voeg toe: 1 extra klasse + 1 extra enumeratie die logisch nodig zijn
  (bv. urgentie/status als enum). Motiveer je keuze.
- Pas ALLE geziene OO-concepten correct toe: static, constructor chaining (:this() en :base()),
  virtual/abstract/override, read-only properties, compositie, pure methodes.

=== WPF (WPFDierenarts) — layout volgens mockup ===
- Linksboven: filters (urgentie, eigenaar, en optie "alleen opgenomen dieren")
- Linksonder: overzicht van dieren (ListBox)
- Rechtsboven: details van het geselecteerde dier + afbeelding van kat of hond
- Rechtsonder: formulier om een nieuw dier toe te voegen

=== ABSOLUUT VERBODEN TECHNIEKEN ===
NIET gebruiken: databinding, DataGrid/GridView/ListView, LINQ, tuples, case guards,
async/await, dynamic, var, expand, anonieme objecten, Invoke, structs, type switches,
user controls, out parameters.
```

---

## 3. Plan van aanpak

### Databankstructuur (DierenartsenDB.sql)
- Tabel `eigenaars`: id (TEXT PK), voornaam, achternaam — 5 rijen
- Tabel `dieren`: id (INT AUTOINCREMENT), naam, eigenaarId (FK), geboortedatum, gewicht,
  urgentie (Laag/Normaal/Spoed), type (Hond/Kat), ras (nullable), isGevaccineerd (nullable),
  isOpgenomen (0/1), datumOpgenomen (nullable) — 10 rijen, 3 opgenomen

### Architectuurbeslissingen
- `Eigenaar.DatabasePad` en `Dier.DatabasePad`: elk hun eigen static property; WPF zet beide bij opstarten
- `InitialiseerDatabase()` in `Dier`: hardcoded SQL (geen afhankelijkheid van extern .sql-bestand op runtime)
- `LeesRij()` private helper in `Dier`: bouwt `Hond` of `Kat` op basis van kolom `type`
- `LaadGefilterd()` bouwt SQL WHERE-clause (geen LINQ, filtering in DB)
- `GeefBasisInfo()` protected in `Dier`: gemeenschappelijke info, aangeroepen door subklassen

### OO-concepten en waar ze voorkomen
| Concept | Locatie |
|---|---|
| `enum` | Urgentie.cs |
| `static` | DatabasePad, LaadAlle(), VoegToe(), Opnemen(), LaadGefilterd() |
| `abstract` | class Dier, method GeefInfo() |
| `override` | ToString() in Dier, GeefInfo() in Hond + Kat |
| `:base()` | Hond + Kat volledige constructor |
| `:this()` | Hond + Kat korte constructor |
| Readonly `{ get; }` | Id, Naam, Ras, IsGevaccineerd, MinAantalTekensRas |
| Compositie | Dier heeft Eigenaar-object |
| `protected` | GeefBasisInfo() in Dier |
| Pure methodes | IsGeldigeNaam(), IsGeldigRas(), GeefInfo(), ToString() |

---

## 4. Gebruikte agent en gespreksverloop

### Agent
**Claude Code (claude-sonnet-4-6)** via CLI/desktop — Anthropic's officiële coding assistant.
Sessiedatum: 2026-06-20.

### Gespreksverloop

| # | Student | Agent |
|---|---|---|
| 1 | Initiële prompt: bouw WPF dierenartspraktijk, werk in plan mode | Agent verkent codebase (SQL-bestand, lege solution), stelt 4 gerichte vragen |
| 2 | Antwoorden: geen mockup, zelf afbeeldingen, Eigenaar+Urgentie akkoord, één formulier | Agent schrijft volledig stappenplan (13 stappen) in plan mode, wacht op goedkeuring |
| 3 | Goedkeuring plan | — |
| 4 | — | **Stap 1**: projecten aangemaakt (WPFDierenarts + CLDierenarts), solution geconfigureerd, NuGet geïnstalleerd, build groen |
| 5 | Solution OK in Visual Studio, ga verder met Stap 2 | **Stap 2**: `Urgentie.cs` aangemaakt (enum Laag/Normaal/Spoed), build groen |
| 6 | Ga verder met Stap 3 | **Stap 3**: `Eigenaar.cs` aangemaakt (Id/Voornaam/Achternaam readonly, DatabasePad static, LaadAlle() SQLite), build groen |
| 7 | Ga verder met Stap 4 | **Stap 4**: `Dier.cs` aangemaakt (abstract class, 2 constructors met :this(), abstract GeefInfo(), override ToString(), DatabasePad, InitialiseerDatabase() met CREATE TABLE + seed), build groen |
| 8 | Ga verder met Stap 5 | **Stap 5**: `GeefBasisInfo()` toegevoegd aan Dier, `Hond.cs` en `Kat.cs` aangemaakt (elk 2 constructors met :base() en :this(), override GeefInfo()), build groen |
| 9 | URGENT: maak claude.md, plan.md, documentatie.md aan | Dit bestand + de twee andere worden aangemaakt |

### Huidige status
Stappen 1 t/m 5 voltooid. Class library heeft: Urgentie, Eigenaar, Dier (abstract), Hond, Kat.
Nog te doen: Stap 6 (data-methodes), Stap 7-13 (WPF).
