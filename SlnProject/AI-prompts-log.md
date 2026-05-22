# AI-prompts log — Dokterspraktijk applicatie (OOAD)

Dit document bevat een chronologische lijst van de prompts die ik aan AI-tools (Claude, ChatGPT, GitHub Copilot Chat) gegeven heb tijdens de ontwikkeling van dit project. Het project bestaat uit een class library `DokterspraktijkLib` en twee WPF-applicaties: `WPFDokter` (portaal voor de dokter) en `WPFPatient` (portaal voor de patiënt). Doel: een complete dokterspraktijk-applicatie met inloggen, beheer van patiënten, afspraken plannen en profielbeheer.

> **Cursus-conventies die ik telkens meegaf aan de AI** (om antwoorden zonder verboden patronen te krijgen):
> *Geen databinding, geen DataGrid/ListView, geen LINQ, geen async/await, geen `var`, geen tuples, geen `dynamic`, geen user controls, geen `out`-parameters. Nederlands commentaar, expliciete loops i.p.v. LINQ.*

---

## Fase 1 — Project setup

Voor ik aan de code begon, moest de solution-structuur opgezet worden: één class library voor de domeinklassen en twee WPF-projecten die er gebruik van maken. Ik wilde dat de dokter- en patiëntenkant volledig gescheiden applicaties zouden zijn maar wel dezelfde domeinmodel zouden delen.

```text
Ik ga een OOAD-project maken in .NET 10 WPF. Welke solution-structuur raad
je aan als ik 2 aparte WPF-apps wil (1 voor dokter, 1 voor patiënt) die
allebei dezelfde domain classes delen (Dokter, Patient, Afspraak,...)?
Class library tussenin?
```

```text
Hoe maak ik in Visual Studio 2022 een nieuwe Solution aan met daarin
3 projecten: een class library "DokterspraktijkLib" en twee WPF apps
"WPFDokter" en "WPFPatient", allemaal .NET 10? Stap voor stap aub.
```

```text
Wat moet ik in de .csproj van een WPF app staan hebben en wat in een
class library? Ik wil zeker zijn dat de lib niet per ongeluk als
executable wordt aangemaakt.
```

```text
Hoe voeg ik vanuit Visual Studio een ProjectReference toe van mijn
WPF app naar de class library? Via "Add > Project Reference" denk ik?
```

---

## Fase 2 — Class library: domeinklassen en overerving

De cursus eist overerving als OOP-concept, dus ik wou een `Persoon` superklasse met `Dokter` en `Patient` als subklasses. Verder enums voor `Geslacht` en `Notificaties`, een `Sessie` static class om de ingelogde gebruiker bij te houden, en een `Hasher` voor SHA256 wachtwoorden.

```text
Ik heb een abstract class Persoon nodig met de gemeenschappelijke
properties: Id (int), Voornaam, Achternaam, Gsm (nullable string),
Email, Paswoord (gehashed), ProfielFotoData (byte[]?). Plus een
helper-property VolledigeNaam die Voornaam + " " + Achternaam
teruggeeft. Schrijf de klasse met Nederlandse commentaar erbij.
```

```text
Schrijf nu een Dokter class die overerft van Persoon. Extra velden:
Rizivnummer (int) en IsGeconventioneerd (bool, in db opgeslagen als
tinyint 0/1). Met een lege constructor en een volledige constructor
die base() oproept.
```

```text
Idem voor Patient: extra velden Geslacht (enum), Geboortedatum
(DateTime) en Notificaties (enum). Ook overerving van Persoon.
```

```text
Maak 2 enums in de lib:
- Geslacht met Man=0, Vrouw=1, X=2
- Notificaties met Geen=0, Mail=1, Sms=2, Beide=3
De int-waardes moeten matchen met wat in de databank staat.
```

```text
Ik wil een Afspraak klasse: Id, Moment (DateTime), Klacht (string),
PatientId, DokterId. En optioneel een Patient en Dokter property
(nullable) zodat ik die kan invullen na een JOIN query. Geen
databinding nodig.
```

```text
Maak een statische Sessie class die de ingelogde gebruiker bijhoudt.
Twee properties: IngelogdeDokter (Dokter?) en IngelogdePatient
(Patient?). Plus een Logout() methode die beide op null zet.
Waarom static? Omdat ik in elke pagina aan deze sessie wil kunnen
zonder ze door te geven.
```

```text
Schrijf een statische Hasher class met één methode Hash(string tekst)
die SHA256 toepast en het resultaat als lowercase hex string
teruggeeft. Met "x2" formatteren in een StringBuilder. Geen LINQ.
```

---

## Fase 3 — CRUD methodes in de class library

De CRUD-methodes horen volgens de cursus in de domeinklassen zelf te zitten (geen aparte DAO laag). Microsoft.Data.SqlClient voor SQL Server. Geen LINQ, dus alle loops expliciet met for-loops en `while (reader.Read())`.

```text
In mijn Dokter class wil ik CRUD methodes:
- static List<Dokter> GeefAlleDokters()
- static Dokter? GeefDokterPerEmail(string email)  voor inloggen
- static Dokter? GeefDokterPerId(int id)
- void Toevoegen()
- void Bijwerken()
- void Verwijderen()  // moet eerst gekoppelde afspraken weghalen
Gebruik SqlConnection + SqlCommand met geparametriseerde queries
(geen string concat want SQL injection). Geen LINQ.
```

```text
Geef me een private helper-methode LeesUitReader(SqlDataReader reader)
die een rij omzet naar een Dokter object. En een ZetParameters(cmd)
helper die de SQL parameters invult voor zowel Insert als Update.
Anders krijg ik code-duplicatie.
```

```text
Doe hetzelfde voor de Patient class: GeefAllePatienten,
GeefPatientPerEmail, GeefPatientPerId, Toevoegen, Bijwerken,
Verwijderen. Bij Verwijderen ook eerst de Afspraak rows weghalen
om foreign key conflicten te voorkomen. Geslacht en Notificaties
moeten als int in de db (cast naar int bij parameter, cast terug
bij lezen).
```

```text
Voor Afspraak heb ik andere queries nodig:
- GeefAfsprakenVoorDokterOpDag(int dokterId, DateTime datum) met JOIN
  naar Patient zodat ik de naam direct heb
- GeefAfsprakenVoorPatient(int patientId) met JOIN naar Dokter
- Toevoegen
- Verwijderen
Bij de READ queries de "lichte" Patient/Dokter property invullen met
enkel id en naam (rest hebben we niet nodig in de lijst).
```

```text
Hoe doe ik in T-SQL "geef alle afspraken op deze specifieke dag, niet
qua uur". CAST(moment AS date) = CAST(@datum AS date)?
```

```text
GSM in de databank is nchar(10) en geeft padded spaties terug bij het
lezen. Hoe trim ik dat enkel als het niet null is in de reader?
```

---

## Fase 4 — Database connectie

LocalDB op mijn eigen machine, één centrale Database class met de connection string. Zo hoef ik niet in elke methode het connection-string-pad te herhalen.

```text
Schrijf een statische Database class met:
- een ConnectionString property (default naar mijn LocalDB:
  Server=zakaria\sqlexpress;Database=DokterspraktijkDB;Trusted_Connection=True;
  TrustServerCertificate=True;)
- een GetConnection() methode die een nieuwe SqlConnection teruggeeft
Caller is verantwoordelijk voor close/dispose via een using-block.
```

```text
Welke NuGet package moet ik installeren voor SqlConnection in .NET 10?
Microsoft.Data.SqlClient of System.Data.SqlClient? En in welk
project — alleen de lib, of ook in de WPF apps?
```

```text
Geef me het CREATE TABLE script voor:
- Dokter (id PK identity, voornaam, achternaam, gsm nchar(10) nullable,
  email, paswoord, profielfotodata image nullable, rizivnummer int,
  isgeconventioneerd tinyint)
- Patient (id PK identity, voornaam, achternaam, geslacht int, gsm,
  email, paswoord, geboortedatum datetime, profielfotodata image,
  notificaties int)
- Afspraak (id PK identity, moment datetime, klacht, patient_id FK,
  dokter_id FK)
```

```text
Mijn wachtwoorden in de db zijn SHA256 hex strings. Geef me een SQL
INSERT statement voor een test-dokter met email "test@dokter.be" en
wachtwoord "test1234" (jij hashed het). Zelfde voor een test-patient.
```

---

## Fase 5 — WPFDokter app: MainWindow + navigatie

De cursus eist één MainWindow met een Frame waarin Pages worden geladen. Linkse navigatiebalk met knoppen, bovenaan een header met naam + profielfoto van de ingelogde gebruiker. Geen User Controls dus alles via standaard WPF.

```text
Ik wil in WPFDokter één MainWindow.xaml met deze layout:
- bovenaan een donkere header (80px hoog) met titel links, en rechts
  de ingelogde naam + ronde profielfoto (50x50)
- links een verticale navigatiebalk (220px breed) met 5 knoppen:
  Start, Login, Afspraken, Patiënten, Uitloggen
- de rest is een Frame met x:Name="mainFrame" waarin ik mijn Pages laad
Geen databinding. Gebruik Grid met ColumnDefinitions en RowDefinitions.
```

```text
Hoe maak ik een ronde profielfoto in WPF zonder user control en zonder
clipping libraries? Border met CornerRadius=25 + ClipToBounds=True +
Image binnenin met Stretch=UniformToFill?
```

```text
Maak een MainWindow.xaml.cs met:
- een static Instance property zodat pagina's kunnen navigeren via
  MainWindow.Instance.mainFrame.Navigate(...)
- in de constructor: mainFrame navigeren naar new StartPage()
- een UpdateNaLogin() methode die na inloggen de naam + foto invult
  en de Afspraken/Patiënten/Uitloggen knoppen activeert (IsEnabled=true)
- click handlers voor alle nav knoppen
- btnUitloggen reset Sessie + UI en navigeert terug naar StartPage
Hoe converteer ik byte[] uit de db naar een BitmapImage voor de
profielfoto?
```

```text
Maak een simpele StartPage.xaml met een titel en wat uitleg-tekst
over de applicatie. Geen logica nodig, gewoon een welcome screen.
```

---

## Fase 6 — WPFDokter: Login + Afspraken

Login moet email + wachtwoord checken via SHA256 hash. Afsprakenpagina moet een kalender tonen waarop je een datum kiest en de afspraken voor die dag ziet, met de mogelijkheid er een te annuleren.

```text
Maak een LoginPage.xaml met:
- TextBlock titel "Inloggen dokter"
- TextBox voor email (x:Name="txtEmail")
- PasswordBox voor wachtwoord (x:Name="txtPaswoord")
- Een TextBlock voor foutmeldingen (x:Name="txtFoutmelding", rood)
- Een Login knop
Foutmeldingen TONEN IN DE TEXTBLOCK, niet in een MessageBox
(cursus-regel: MessageBox enkel voor confirmaties, niet voor errors).
```

```text
Code-behind voor login:
1. Form checking: email + paswoord ingevuld, email moet '@' bevatten
2. Dokter ophalen via Dokter.GeefDokterPerEmail(email)
3. SHA256 hash van ingegeven paswoord vergelijken met dokter.Paswoord
4. Bij succes: Sessie.IngelogdeDokter = dokter, MainWindow.UpdateNaLogin()
   en navigeer naar PatientenOverzichtPage
5. Try/catch rond db-call met foutmelding in de TextBlock
```

```text
Ik wil een AfsprakenPage met:
- Links een Calendar control (x:Name="kalender")
- Rechts: TextBlock "Afspraken op [datum]" en een ListBox met de
  afspraken (HH:mm - patiëntnaam)
- Onderaan: TextBlock met de reden van de geselecteerde afspraak +
  een knop "Annuleren afspraak"
Bij datum selecteren: laad afspraken via
Afspraak.GeefAfsprakenVoorDokterOpDag(dokterId, datum).
Geen databinding, items toevoegen via lstAfspraken.Items.Add().
```

```text
Annuleer-knop moet eerst MessageBox.Show met YesNo vragen om
bevestiging. Bij Yes: gekozen.Verwijderen() en de lijst herladen.
Bij errors: tonen in txtFoutmelding (TextBlock), niet in MessageBox.
```

---

## Fase 7 — WPFDokter: patiëntenbeheer

Patiëntenoverzicht moet getoond worden als cards in een grid (zoals de cursus-demo `SlnDemoItemsPanel`). De cards moeten dynamisch in code-behind gemaakt worden (geen databinding, geen ItemsControl met DataTemplate). Plus zoekbalk, en knoppen voor Details, Wijzig, Verwijder, Nieuw.

```text
PatientenOverzichtPage moet de patiënten tonen als cards (260x320),
in een WrapPanel zodat ze mooi wrappen. Plus bovenaan:
- TextBox zoekbalk (txtZoek)
- knop "+ Nieuwe patiënt"
- TextBlock voor foutmelding
ScrollViewer rond de WrapPanel voor als er veel patiënten zijn.
XAML alleen, code-behind komt zo.
```

```text
Code-behind: bouw de cards dynamisch op in een private methode
MaakPatientCard(Patient p) die een Border teruggeeft. Inhoud:
- ronde profielfoto (80x80) bovenaan
- naam (bold), email en gsm onder elkaar
- 3 knoppen onderaan naast elkaar: Details (zwart), Wijzig (grijs),
  Verwijder (rood)
De Click handlers krijgen de patient via een closure
(delegate (s, e) { GaNaarDetails(patient); }).
Hoe geef ik kleuren mee als hex-codes via SolidColorBrush in C#?
```

```text
Zoekfunctie: bij txtZoek_TextChanged filter de allePatienten lijst op
voornaam of achternaam (case-insensitive Contains). Maak een nieuwe
List<Patient> en bouw de cards opnieuw op. Geen LINQ, gewoon een
for-loop met if.
```

```text
Verwijder-knop: bevestiging via MessageBox.Show YesNo. Bij Yes:
patient.Verwijderen() en LaadPatienten() opnieuw. Errors in de
foutmelding TextBlock.
```

```text
PatientDetailsPage: krijgt een Patient via constructor en toont alle
velden read-only (naam, email, gsm, geslacht, geboortedatum,
notificaties + grote profielfoto). Plus een "Terug" knop naar het
overzicht. Geslacht en Notificaties tonen met .ToString() op de enum
(geeft "Man" / "Beide" enz.).
```

```text
PatientBewerkenPage: één pagina die zowel "nieuw" als "wijzig" doet.
Constructor neemt Patient? — null = nieuwe, niet-null = wijzig.
Velden:
- TextBoxes voor voornaam, achternaam, gsm, email
- PasswordBox voor wachtwoord (bij wijzig: leeg = behouden, hint
  tonen onder het veld)
- ComboBox geslacht (Man/Vrouw/X) gevuld via XAML ComboBoxItem
- DatePicker voor geboortedatum
- ComboBox notificaties (Geen/Mail/Sms/Beide)
- Knop "Kies foto" + Image preview
- Knoppen Opslaan en Terug
Form checking met returns bij elke fout (verplicht, geldig email,
max 10 tekens gsm, etc.). Bij Save: Toevoegen() of Bijwerken() in
try/catch.
```

```text
Voor het kiezen van een profielfoto: OpenFileDialog met filter voor
.jpg, .jpeg, .png. Bytes inlezen via File.ReadAllBytes en in
huidigeFotoData zetten. Dan tonen via een BitmapImage met MemoryStream
en CacheOption.OnLoad.
```

---

## Fase 8 — WPFPatient app

Vergelijkbare structuur als WPFDokter, maar met andere pages: login, afspraken-overzicht (verleden vs toekomstig), nieuwe afspraak boeken, profiel bekijken/bewerken.

```text
Maak in WPFPatient een MainWindow.xaml die heel hard lijkt op die van
WPFDokter: zwarte header met titel "Dokterspraktijk - Patiënt Portaal",
linkse nav met knoppen Start / Login / Afspraken / Profiel / Uitloggen,
en een Frame in het midden. Zelfde static Instance pattern + UpdateNaLogin().
```

```text
Patient LoginPage: zelfde flow als de dokter-login maar dan met
Patient.GeefPatientPerEmail en Sessie.IngelogdePatient. Na inloggen
navigeer naar AfsprakenOverzichtPage (volgens de opgave).
```

```text
AfsprakenOverzichtPage voor patient:
- titel "Mijn afspraken"
- knop "+ Nieuwe afspraak"
- ListBox met alle afspraken via Afspraak.GeefAfsprakenVoorPatient
- Items in formaat "[dd MMM yyyy HH:mm] - Dr. naam"
- Toekomstige afspraken normaal, verlopen ervoor "[VERLOPEN]" tag
- Bij selectie: txtReden tonen
- Annuleren knop: ENKEL enabled als a.Moment > DateTime.Now
  (verlopen afspraken mag je niet meer annuleren)
- Bij Annuleren: MessageBox YesNo + Verwijderen()
```

```text
NieuweAfspraakPage:
- Links: ListBox lstDokters met alle dokters ("Dr. " + VolledigeNaam)
- Onder de listbox: zodra geselecteerd, toon de dokter zijn info
  (naam, email, gsm, riziv, geconventioneerd ja/nee)
- Rechts: DatePicker (DisplayDateStart = vandaag), ComboBox tijdsblokken
  (08:00 t/m 17:30 elk half uur), TextBox voor reden
- Knoppen Bevestigen en Terug
Op Bevestigen: form checken, DateTime samenstellen uit datum + uur:minuut
(string.Split(':') + int.Parse), check niet in het verleden, dan
nieuwe Afspraak.Toevoegen() en terug naar overzicht.
Hoe vul ik de tijdsblokken? Met een for-loop van 8 tot 18 en
"hh:00" + "hh:30" per uur.
```

```text
ProfielPage: toont read-only de profielgegevens van de ingelogde
patient. Belangrijk: bij het binnenkomen op de pagina opnieuw uit de db
laden (Patient.GeefPatientPerId(sessie.Id)) zodat wijzigingen uit de
bewerk-pagina ook zichtbaar zijn. Daarna ook Sessie.IngelogdePatient
updaten. Plus knop "Bewerk profiel".
```

```text
ProfielBewerkenPage: zelfde fields als PatientBewerkenPage van de
dokter-app, maar nu voor de eigen ingelogde patient. Geen "nieuwe"
flow nodig — alleen wijzigen. Na opslaan: Sessie updaten + 
MainWindow.UpdateHeader() oproepen (naam/foto in de header kan
gewijzigd zijn). Navigeer terug naar ProfielPage.
```

---

## Fase 9 — Afwerking en opkuis

Kleine afwerkingsvragen die tijdens het bouwen opkwamen.

```text
Hoe rond ik een Border in WPF? CornerRadius="8" werkt, maar hoe maak
ik er een schaduw onder met enkel WPF en zonder libraries?
```

```text
Wat is het verschil tussen TextBox.Text en PasswordBox.Password? Waarom
geen .Text op een PasswordBox?
```

```text
Geef me een korte uitleg voor in mijn verslag: wat is het verschil
tussen abstract class en interface in C#? Ik gebruik abstract Persoon
omdat Dokter en Patient gemeenschappelijke property implementatie
delen — een interface zou enkel de signatures forceren.
```

```text
Waarom static op de Sessie class? Geef me 2-3 zinnen voor in mijn
verslag waarom ik dit zo gekozen heb.
```

```text
Hoe formatteer ik een DateTime als "dd MMMM yyyy" met de Nederlandse
maand-naam? Moet ik culture meegeven of pakt hij dat automatisch?
```

---

## Fase 10 — Foutoplossing (echte debug-sessie)

Op een bepaald moment werkte het project niet meer: build zei "succeeded" maar bij F5 startte er geen app op. Onderstaande prompts zijn de échte debug-sessie met de AI die het uiteindelijk opgelost heeft.

```text
Mijn WPF-project voor het vak OOAD bouwt succesvol (Build succeeded),
maar wanneer ik op Run/Start druk gebeurt er niets of start de app
niet op.

CONTEXT:
- Solution met 3 projecten: class library DokterspraktijkLib +
  WPFDokter app + WPFPatient app (.NET 10 WPF).
- Ik heb recent bestanden verplaatst naar de juiste projecten/mappen,
  dus er kan iets misgelopen zijn met namespaces, project references
  of csproj.

WAT IK VRAAG - check dit systematisch:
1. STARTUP PROJECT: is er een startup project ingesteld? Een class
   library kan NIET het startup project zijn.
2. APP.XAML: controleer of de StartupUri naar een bestaand venster
   verwijst.
3. MAINWINDOW: bestaat er in elk WPF project exact één hoofdvenster?
   Check of de x:Class overeenkomt met de namespace + klasse-naam.
4. NAMESPACES: door verplaatsen kan de namespace nu fout staan.
5. PROJECT REFERENCES: verwijzen de WPF apps correct naar de lib?
6. CSPROJ: controleer of de .csproj nog klopt.
7. BUILD vs RUN: stale build cache? Clean Solution + Rebuild.
Begin met stap 1 t/m 3 en rapporteer wat je vindt voor je iets aanpast.
```

```text
Goede analyse. Ga nu door met de volledige reparatie. Los alle 5
problemen (A t/m E) op zodat het project correct bouwt en draait.

NAMESPACE-KEUZE (probleem E):
Standaardiseer alles naar "Dokterspraktijk" met kleine p - dat is
de correcte Nederlandse spelling. Hernoem het lib-project, de map,
de csproj en de assembly van "DoktersPraktijkLib" naar
"DokterspraktijkLib".

STAP 4 - Class library herstellen:
- Verwijder <OutputType>WinExe</OutputType> en <UseWPF>true</UseWPF>.
- Verwijder de spook-bestanden uit de lib: App.xaml, App.xaml.cs,
  MainWindow.xaml, MainWindow.xaml.cs.

STAP 5 - ProjectReferences toevoegen:
- Voeg in WPFDokter.csproj en WPFPatient.csproj een ProjectReference
  toe naar DokterspraktijkLib.csproj.

STAP 6 - Namespaces overal nakijken.

STAP 7 - Clean + Rebuild:
- Verwijder alle bin/ en obj/ mappen.
- Doe een volledige rebuild.
- Geef me de exacte lijst van eventuele errors met bestand + regel.
- Los die ook op tot de build 100% schoon is.
```

```text
VS dicht, ga door
```

```text
Hoe stel ik in Visual Studio het startup project in? Ik wil tussen
WPFDokter en WPFPatient kunnen kiezen, en ook beide tegelijk kunnen
opstarten voor demo's.
```

---

## Fase 11 — Documentatie

```text
Maak een document met alle prompts die gebruikt werden om dit project
te maken, voor mijn AI-documentatie van het vak OOAD. Groepeer
chronologisch per ontwikkelfase, in Nederlands, elke prompt in een
apart code-block. Voeg ook de echte debug-prompts toe die we recent
gebruikten (namespace-problemen, class library configuratie).
```

---

## Reflectie

De AI was vooral nuttig voor:
- **Boilerplate-code wegnemen** (CRUD-patronen, BitmapImage-helpers, OpenFileDialog setup).
- **Cursus-conventies respecteren** door ze elke keer mee te geven in de prompt — anders kreeg ik LINQ of databinding terug.
- **Structurele debugging** zoals de namespace + csproj mess in fase 10, die ik zelf niet snel had gevonden omdat "Build succeeded" misleidend was.

Waar ik moest opletten:
- De AI gebruikt standaard graag LINQ, `var`, databinding en async/await — telkens expliciet verbieden in de prompt of laten herschrijven.
- Foutmeldingen in MessageBox in plaats van TextBlock — moest ik telkens corrigeren.
- Sommige voorgestelde features (DataGrid, ItemsControl met DataTemplate) zijn elegant maar verboden in de cursus; ik moest dan vragen "kan dit zonder databinding, dynamisch in code-behind".
