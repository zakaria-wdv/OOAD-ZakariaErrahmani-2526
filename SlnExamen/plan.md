# Stappenplan — WPF Dierenartspraktijk OOAD 25-26

## Projectstructuur
```
SlnExamen/
├── SlnExamen.slnx
├── DierenartsenDB.sql
├── CLDierenarts/          (.NET 10, Class Library)
│   ├── Urgentie.cs
│   ├── Eigenaar.cs
│   ├── Dier.cs
│   ├── Hond.cs
│   ├── Kat.cs
│   └── DierValidator.cs
└── WPFDierenarts/         (.NET 10, WPF App)
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    └── Resources/
        ├── kat.png
        └── hond.png
```

## WPF Layout (4-kwadrant)
```
+──────────────────────────+──────────────────────────────+
│ FILTERS                  │ DETAILS                      │
│ Urgentie: [ComboBox ▼]   │ Type / Naam / Ras / ...      │
│ Eigenaar: [ComboBox ▼]   │ [afbeelding hond/kat]        │
│ [✓] Alleen opgenomen     │ [Dier Opnemen] (disabled     │
│ [Filteren]               │  als al opgenomen)           │
+──────────────────────────+──────────────────────────────+
│ LIJST (ListBox)          │ NIEUW DIER TOEVOEGEN         │
│ - Bobbie                 │ Type / Naam / Eigenaar /     │
│ - Nala (Opgenomen)       │ Geboortedatum / Gewicht /    │
│ - Max (Opgenomen)        │ Urgentie / Ras of Gevaccineerd│
│ - ...                    │ [Toevoegen] [fout TextBlock] │
+──────────────────────────+──────────────────────────────+
```

## Stappen

### ✅ Stap 1 — Project setup
- WPFDierenarts (.NET 10 WPF) + CLDierenarts (.NET 10 classlib) aangemaakt
- Beide toegevoegd aan SlnExamen.slnx
- Project reference WPFDierenarts → CLDierenarts
- NuGet Microsoft.Data.Sqlite geïnstalleerd in CLDierenarts
- Build: 0 errors

### ✅ Stap 2 — Enum Urgentie
- CLDierenarts/Urgentie.cs: `public enum Urgentie { Laag, Normaal, Spoed }`

### ✅ Stap 3 — Klasse Eigenaar
- CLDierenarts/Eigenaar.cs
- Properties: Id, Voornaam, Achternaam (allen readonly { get; })
- static string DatabasePad { get; set; }
- Eigenaar(string id, string voornaam, string achternaam)
- override ToString() → "Voornaam Achternaam"
- static List<Eigenaar> LaadAlle() — SQLite SELECT

### ✅ Stap 4 — Abstracte klasse Dier + InitialiseerDatabase
- CLDierenarts/Dier.cs
- Properties: Id, Naam, Eigenaar, Geboortedatum, Gewicht, Urgentie, IsOpgenomen, DatumOpgenomen
- Volledige constructor (8 params) + korte constructor (:this())
- abstract GeefInfo() + protected GeefBasisInfo()
- override ToString() → "Naam" of "Naam (Opgenomen)"
- static DatabasePad, static InitialiseerDatabase() (CREATE TABLE IF NOT EXISTS + seed)

### ✅ Stap 5 — Klasse Hond + Kat
- CLDierenarts/Hond.cs: Ras { get; }, 2 constructors (:base() en :this()), override GeefInfo()
- CLDierenarts/Kat.cs: IsGevaccineerd { get; }, 2 constructors (:base() en :this()), override GeefInfo()

### ⬜ Stap 6 — Data-methodes in Dier
- static List<Dier> LaadAlle()
- static List<Dier> LaadGefilterd(Urgentie? urgentie, string eigenaarId, bool alleenOpgenomen)
- static void VoegToe(Dier dier)
- static void Opnemen(int id)
- private static Dier LeesRij(SqliteDataReader reader, List<Eigenaar> eigenaars)

### ⬜ Stap 7 — WPF XAML layout (MainWindow.xaml)
- Grid 2 kolommen: links (filters + ListBox), rechts (details + formulier)
- Alle controls benoemd

### ⬜ Stap 8 — WPF opstarten + dieren laden
- Window_Loaded: DatabasePad instellen, InitialiseerDatabase(), ComboBoxes vullen, lijst laden
- LaadDierenInLijst(): ListBox vullen via Items.Add(new ListBoxItem { Content=..., Tag=... })

### ⬜ Stap 9 — Details tonen bij selectie
- lbDieren_SelectionChanged: Tag ophalen als Dier, GeefInfo() tonen, afbeelding instellen,
  btnOpnemen enable/disable

### ⬜ Stap 10 — Filters toepassen
- btnFilter_Click: LaadGefilterd() aanroepen, ListBox herladen

### ⬜ Stap 11 — Nieuw dier toevoegen
- cbType_SelectionChanged: Ras-veld en Gevaccineerd-checkbox tonen/verbergen
- btnToevoegen_Click: validatie via DierValidator, Hond of Kat aanmaken, VoegToe(), herladen

### ⬜ Stap 12 — Dier opnemen
- btnOpnemen_Click: Dier.Opnemen(id), lijst herladen

### ⬜ Stap 13 — Afbeeldingen
- kat.png + hond.png als Resource in WPFDierenarts
- BitmapImage via pack://application URI

## Klasse DierValidator (toe te voegen in Stap 6 of 11)
- int MinAantalTekensRas { get; } = 3  (readonly auto-property)
- bool IsGeldigeNaam(string naam)  → foreach: char.IsLetter || ' ' || '-'
- bool IsGeldigRas(string ras)  → ras.Length >= MinAantalTekensRas

## Verificatie end-to-end
1. dotnet build → 0 errors
2. App starten → 10 dieren laden, 3 met "(Opgenomen)"
3. Selecteer "Nala" → kat-details, afbeelding kat, knop Opnemen disabled
4. Filter Urgentie = Spoed → 3 dieren
5. Filter Eigenaar = Jonas De Smet → 2 dieren
6. Voeg hond toe, ras "La" → fout "Ras te kort"
7. Voeg hond correct toe → verschijnt in lijst
8. Selecteer "Buddy" → klik Opnemen → "(Opgenomen)" in lijst, knop disabled
