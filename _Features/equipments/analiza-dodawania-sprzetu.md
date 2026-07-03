# Analiza: dodawanie nowego sprzętu do bazy (Equipment — Add)

Analiza przepływu "Add equipment" w `AssetSquirrelAuthorize.WebApp` — od kliknięcia
przycisku na liście, przez dialog, use case, aż po zapis w bazie.

## Zakres analizy (pliki)

- `Components/Pages/Equipment/Equipment.razor` — lista + wywołanie dialogu Add
- `Components/Pages/Equipment/EquipmentAddDialogBox.razor` — formularz dodawania
- `Components/Pages/Equipment/EquipmentEditDialogBox.razor` — porównawczo, bo część
  błędów jest skopiowana 1:1 z Add do Edit
- `AssetSquirrel.UseCases/EquipmentUseCase/AddEquipmentUseCase.cs`
- `AssetSquirrel.UseCases/PluginInterfaces/IEquipmentRepository.cs`
- `AssetsSquirrel.Plugins.EFCoreSqlServer/Repositories/EquipmentRepository.cs`
- `AssetSquirrel.CoreBusiness/Equipment.cs`, `Dto/EquipmentDto.cs`
- `Components/Pages/EquipmetAssignment/EquipmentAssignment.razor` (dla kontekstu)

## Jak to działa dziś (przepływ)

1. Użytkownik klika "Add equipment" na `/equipment` → otwiera się `DialogBox` z
   `EquipmentAddDialogBox`.
2. Dialog w `OnInitializedAsync` ładuje listy Suppiler/Manufacturer/HardwareType
   (tylko aktywne, `IsActive == true`) i Invoice (wszystkie, bez filtra) do
   dropdownów, oraz **ustawia `Equipment.UserId` na ID aktualnie zalogowanego
   użytkownika** — zgodnie z zamierzonym działaniem to pole oznacza osobę,
   która wprowadziła sprzęt do systemu (rejestrującego), **nie** osobę, która
   fizycznie posiada/użytkuje dany sprzęt (patrz wyjaśnienie przy pkt. 1 niżej).
3. Formularz pokazuje 4 dropdowny (Suppiler, Manufacturer, HardwareType, Invoice)
   i pola tekstowe (ModelName, SerialNumber, Description) z ręcznie rysowanymi
   ikonami "✓/⚠" obok każdego pola jako podpowiedź walidacji.
4. Klik "Zapisz" → `EditContext.Validate()` (DataAnnotations) → jeśli OK,
   `AddEquipmentUseCase.AddEquipmentAsync(dto)` → `Adapt<Equipment>()` →
   `EquipmentRepository.AddEquipmentAsync` → `dbContext.Equipments.Add(...)` +
   `SaveChanges()` → `Result<Equipment>`.
5. Wynik (`.Success` jako `bool`) wraca do `Equipment.razor`, które pokazuje toast
   "Equipment has been added." / "Saving error." i odświeża listę.

## Znalezione problemy

### ✅ Wyjaśnione — nie jest błędem

**`Equipment.UserId` = osoba, która wprowadziła sprzęt do systemu, nie osoba,
która go posiada/użytkuje.** Pierwotna wersja tej analizy błędnie zakładała, że
automatyczne ustawianie `Equipment.UserId` na zalogowanego użytkownika w
`EquipmentAddDialogBox` i `EquipmentEditDialogBox` to nieumyślne "przypisanie
właściciela". **Potwierdzone przez właściciela projektu: to zamierzone
działanie** — pole oznacza rejestrującego/wprowadzającego rekord, a nie
posiadacza sprzętu. Osobna, niedokończona strona `/equipmentassignment` (pusty
stub — `<h3>Equipment Assignment</h3>`, brak logiki) to prawdopodobnie miejsce
na faktyczne przypisanie sprzętu do pracownika, gdy zostanie zaimplementowane —
i jest z założenia czymś innym niż `Equipment.UserId`.

Zostaje z tego jednak drobna obserwacja (patrz pkt. 8 niżej w sekcji Drobne):
skoro nawet przy analizie kodu ta semantyka nie była oczywista, warto ją
udokumentować/nazwać jaśniej w samym kodzie.

### 🔴 Krytyczne

**1. ✅ Naprawione — walidacja wymaganych dropdownów (Suppiler/Manufacturer/
HardwareType) była czysto kosmetyczna, nie blokowała zapisu.**
`SuppilerId`, `ManufacturerId`, `HardwareTypeId` w `EquipmentDto` miały atrybut
`[Required]`, ale były typu `int` (nie `int?`) — `[Required]` na nienullowalnym
typie wartościowym w ASP.NET Core DataAnnotations nigdy nie zwracał błędu (`0`,
czyli placeholder "…suppiler…" w dropdownie, był traktowany jako poprawna
wartość). Naprawione: pola zmienione na `int?` w `EquipmentDto`, placeholdery
dropdownów (`<option value="">`) wiążą się teraz z `null` zamiast `0`, a
`EditContext.Validate()` poprawnie odrzuca formularz, gdy dropdown pozostał
niewybrany — w obu dialogach (Add i Edit). Ikony ✓/⚠ sprawdzają teraz
`is null` zamiast `== 0`. Dodatkowo `OnSave` w obu dialogach przekazuje teraz
cały `Result<EquipmentDto>` (zamiast samego `bool`) do `Equipment.razor`, które
pokazuje `result.Message` w toaście błędu zamiast generycznego "Saving
error." — użytkownik widzi realny powód niepowodzenia zapisu (np. komunikat
wyjątku EF przy naruszeniu klucza obcego), a nie tylko sam fakt, że coś poszło
nie tak.

**2. ✅ Naprawione — przycisk "Usuń" (kosz) na liście sprzętu był martwy.**
`Equipment.razor` → `RemoveEquipment(...)` miał całą logikę zakomentowaną i
odwoływał się do nieistniejącej metody `RemoveEquipmentAsync`. Naprawione:
zgodnie z decyzją właściciela projektu, sprzęt **nigdy nie jest fizycznie
usuwany z bazy** — przycisk ustawia teraz `IsActive = false` i zapisuje przez
`ViewEquipmentUseCase.UpdateEquipmentAsync(equipment)` (ten sam wzorzec co
"dezaktywacja" w Employees/Locations/Manufacturers), po czym odświeża listę.
Tooltip przycisku zmieniony z "Remove" na "Dezaktywuj sprzęt." dla jasności —
ikona kosza pozostała bez zmian (deaktywacja usuwa pozycję z domyślnego,
aktywnego widoku listy, więc wizualny efekt dla użytkownika jest podobny do
usunięcia, ale rekord i historia zostają zachowane w bazie).

**3. ✅ Naprawione — zmiany w `Equipments` nigdy nie były zapisywane do
`EquipmentHistories`.** Tabela/encja `EquipmentHistory` istniała w schemacie
(migracja, konfiguracja EF, `DbSet<EquipmentHistory>`), ale żaden kod
(repozytorium, use case, UI) nigdy do niej nic nie zapisywał — kompletnie
martwa tabela audytu, mimo że wygląda jak w pełni wdrożona funkcja. Naprawione:
`EquipmentRepository.AddEquipmentAsync`/`UpdateEquipmentAsync` tworzą teraz
snapshot bieżącego stanu `Equipment` (wszystkie pola + `DateOfChange` + osoba
wykonująca operację jako `ApplicationUserId`) i zapisują go do
`EquipmentHistories` w tej samej transakcji co zmiana w `Equipments`. Dotyczy
to też "dezaktywacji" sprzętu (pkt 2 wyżej), bo to zwykły `Update` — dodatkowo
poprawiłem `Equipment.razor`, żeby przy kliknięciu "Dezaktywuj" ustawiało
`equipment.UserId` na aktualnie zalogowanego użytkownika (wcześniej ta strona
w ogóle nie miała dostępu do tożsamości użytkownika), tak żeby historia
poprawnie wskazywała, kto wykonał deaktywację, a nie kto ostatnio
dodał/edytował dany sprzęt.
**Zastrzeżenie:** `DeleteEquipmentAsync` (fizyczne usunięcie z bazy) też
dostał zapis do historii, ale jest on i tak bezużyteczny w tym przypadku —
`EquipmentHistoryConfiguration` ma `OnDelete(DeleteBehavior.Cascade)` z
`Equipment` do `EquipmentHistories`, więc usunięcie wiersza `Equipment`
kasuje w bazie *całą* jego historię, łącznie ze świeżo dopisanym wpisem
"usunięcia". Trwały ślad audytowy usunięcia wymagałby zmiany zachowania
kaskady (osobna migracja) — nieistotne praktycznie, bo `DeleteEquipmentAsync`
nie jest dziś wywoływane z żadnego miejsca w UI (sprzęt jest tylko
dezaktywowany, zgodnie z pkt 2).

### 🟠 Ważne

**4. ✅ Naprawione — pole "Manufacturer" miało etykietę "Suppiler" (kopiuj-wklej),
w obu dialogach Add i Edit.** Dropdown był funkcjonalnie poprawnie spięty z
`Equipment.ManufacturerId`, ale użytkownik widział dwa pola nazwane "Suppiler"
pod rząd. Etykieta zmieniona na "Manufacturer" w obu dialogach.

**5. ✅ Naprawione — ikona walidacji przy "Invoice number" sprawdzała
`HardwareTypeId`, nie `InvoiceId`** (kopiuj-wklej), w obu dialogach. Zmieniona
na sprawdzanie `Equipment.InvoiceId` — podpowiedź ✓/⚠ przy fakturze reaguje
teraz na wybór faktury, nie typu sprzętu.

**6. ✅ Naprawione — brak zabezpieczenia przed duplikatem numeru seryjnego.**
Ani UI, ani `AddEquipmentUseCase`, ani baza nie chroniły przed dodaniem dwóch
sztuk sprzętu z tym samym numerem seryjnym. Naprawione na poziomie
`EquipmentRepository` (wspólny punkt dla Add/Edit/dezaktywacji, bo wszystkie
trzy przechodzą przez `AddEquipmentAsync`/`UpdateEquipmentAsync`):
`AddEquipmentAsync` odrzuca zapis, jeśli `SerialNumber` już istnieje w bazie;
`UpdateEquipmentAsync` robi to samo, ale wyklucza z porównania edytowany
rekord (`EquipmentId != equipment.EquipmentId`), żeby zapis bez zmiany numeru
seryjnego nie kolidował sam ze sobą. Komunikat trafia do `Result.Fail(...)` i
jest teraz widoczny w UI (patrz pkt 1 wyżej). **Celowo pominięty** unique index
w bazie danych — wymagałby migracji na żywej bazie bez uprzedniego
sprawdzenia, czy nie ma już w niej duplikatów `SerialNumber`; sam check w
repozytorium wystarcza dla realnego profilu ryzyka tej aplikacji (wewnętrzne
narzędzie administracyjne, brak współbieżnych zapisów). Do rozważenia jako
osobny krok, jeśli kiedyś zajdzie potrzeba twardej gwarancji na poziomie bazy.

**7. ✅ Naprawione — pole `IsAddedToWarehouse` (na encji `Equipment`) nie było
w ogóle wystawione w formularzu.** Zawsze zapisywane jako `false` (wartość
domyślna DTO), niezależnie od rzeczywistego stanu. Dodano checkbox "Added to
warehouse?" w obu dialogach (Add i Edit), analogicznie do istniejącego
checkboxa "Is active?" w Edit.

### 🟡 Drobne

**8. ✅ Naprawione — nazwa `Equipment.UserId`/kolumna "User" na liście
sugerowała "właściciela" sprzętu, a oznacza osobę rejestrującą.** Przemianowane
na `Equipment.RegisteredByUserId`/`RegisteredByUser` (kolumna DB pozostała
`UserId` dzięki `[Column("UserId")]`, więc zmiana nie wymagała migracji) oraz
`EquipmentDto.RegisteredByUserId`/`RegisteredByUserName`. Nagłówek kolumny na
liście zmieniony z "User" na "Added by". Zaktualizowano wszystkie miejsca
odwołujące się do starej nazwy: `EquipmentRepository`, `EquipmentConfiguration`,
`Equipment.razor`, `EquipmentAddDialogBox.razor`, `EquipmentEditDialogBox.razor`.

**9. ✅ Naprawione — myląca nazwa zmiennej `isLoading` w `EquipmentEditDialogBox`,
semantyka odwrócona względem `EquipmentAddDialogBox`.** Ujednolicone: `isLoading`
w obu dialogach ma teraz identyczne znaczenie i strukturę — `true` na starcie
("trwa ładowanie"), `false` po załadowaniu list Suppiler/Manufacturer/
HardwareType/Invoice, formularz renderuje się w gałęzi `else` (`@if (isLoading)
{ spinner } else { formularz }`).

**10. `dbContext.SaveChanges()` (synchroniczne) zamiast `SaveChangesAsync()` w
`AddEquipmentAsync`/`UpdateEquipmentAsync`.** ✅ Naprawione przy okazji dodawania
zapisu do `EquipmentHistories` (pkt 3) — obie metody używają teraz
`await dbContext.SaveChangesAsync()`, spójnie z `DeleteEquipmentAsync`.

**11. ✅ Naprawione — brak wskaźnika ładowania w `EquipmentAddDialogBox`.**
Pusty blok `if(isLoading) { }` zastąpiony spinnerem Bootstrapa
(`spinner-border`, wyśrodkowany). Ten sam spinner dodany też do
`EquipmentEditDialogBox` przy okazji ujednolicania `isLoading` (pkt 9) —
wcześniej Edit też pokazywał pusty dialog podczas ładowania list.

**12. Niespójność walidacji "Model name" między DataAnnotation a ikoną
podpowiedzi.** `[MinLength(5)]` na `ModelName` oznacza, że długość **5** jest
już poprawna, ale ikona w UI sprawdza `ModelName.Length <= 5` (czyli traktuje
5 znaków jako wciąż niepoprawne) — podpowiedź wizualna jest bardziej
restrykcyjna niż faktyczna walidacja zapisu.

## Sugestie (priorytet malejący)

1. ✅ **Zrobione** — `Equipment.UserId` przemianowane na `RegisteredByUserId`
   (kolumna DB bez zmian dzięki `[Column("UserId")]`, brak migracji), kolumna
   na liście "User" → "Added by" (patrz pkt 8 w sekcji Drobne).
2. ✅ **Zrobione** — `SuppilerId`/`ManufacturerId`/`HardwareTypeId` w
   `EquipmentDto` zmienione na `int?`, dropdown placeholdery wiążą się z `null`,
   `EditContext.Validate()` teraz faktycznie blokuje zapis przy niewybranym
   dropdownie. `OnSave` w obu dialogach przekazuje `Result<EquipmentDto>` do
   `Equipment.razor`, które pokazuje `result.Message` w toaście błędu zamiast
   generycznego "Saving error." (patrz pkt 1 w sekcji Krytyczne).
3. ✅ **Zrobione** — przycisk "Usuń" sprzętu ustawia teraz `IsActive = false`
   zamiast (nieistniejącego wcześniej) fizycznego usuwania z bazy.
4. ✅ **Zrobione** — `AddEquipmentAsync`/`UpdateEquipmentAsync` zapisują teraz
   snapshot każdej zmiany do `EquipmentHistories` (patrz pkt 3 w sekcji
   Krytyczne). `DeleteEquipmentAsync` też, z zastrzeżeniem opisanym tam.
5. ✅ **Zrobione** — etykieta "Suppiler" przy drugim dropdownie zmieniona na
   "Manufacturer" (Add i Edit).
6. ✅ **Zrobione** — ikona walidacji faktury sprawdza teraz `InvoiceId`, nie
   `HardwareTypeId` (Add i Edit).
7. ✅ **Zrobione** — dodano sprawdzenie unikalności `SerialNumber` w
   `EquipmentRepository` (Add odrzuca duplikat, Update wyklucza edytowany
   rekord z porównania). Unique index w bazie celowo pominięty (patrz pkt 6 w
   sekcji Ważne).
8. ✅ **Zrobione** — `IsAddedToWarehouse` dodane jako checkbox "Added to
   warehouse?" w obu dialogach (Add i Edit).
9. ✅ **Zrobione** — `isLoading` ujednolicone w obu dialogach (ta sama
   semantyka: `true` = trwa ładowanie, formularz w gałęzi `else`).
10. ✅ **Zrobione** — `SaveChanges()` zamienione na `await SaveChangesAsync()`
    w `AddEquipmentAsync`/`UpdateEquipmentAsync` przy okazji pkt 4.
11. ✅ **Zrobione** — dodano spinner (Bootstrap `spinner-border`) na czas
    ładowania list w obu dialogach (Add i Edit).

## ⚠️ Odkryty przy okazji: dryf schematu bazy danych (niezwiązany z powyższym)

Przy weryfikacji, czy przemianowanie `UserId` → `RegisteredByUserId` wymaga
migracji (`dotnet ef migrations has-pending-model-changes`), narzędzie
niespodziewanie zgłosiło pending changes — mimo że sama zmiana nazwy nie
dotyka kolumny DB (`[Column("UserId")]`). Po wygenerowaniu migracji "na
podgląd" okazało się, że przyczyna jest **całkowicie inna i wcześniej
nieznana**: model C# ma dwie kolumny, których faktycznie brakuje w
podłączonej bazie danych:
- `Equipments.IsAddedToWarehouse` (bit, patrz też pkt 7 w sekcji Ważne)
- `EquipmentHandovers.IsPosted` (bit)

`dotnet ef migrations list` potwierdza, że wszystkie 10 wcześniejszych
migracji są już zastosowane w tej bazie — czyli te dwie kolumny zostały
dodane do encji C# w pewnym momencie, ale nigdy nie doczekały się migracji.
Realny skutek: każdy `INSERT`/`UPDATE` na `Equipments` lub
`EquipmentHandovers`, który EF spróbuje wygenerować z pełnym zestawem
kolumn modelu, skończy się błędem SQL "Invalid column name" — to może być
faktyczna przyczyna niepowodzeń zapisu w tym środowisku, niezależna od
błędów opisanych wyżej w tym dokumencie.

Wygenerowano migrację naprawczą `AddMissingEquipmentAndHandoverColumns`
(dodaje obie kolumny z `defaultValue: false`) — **na razie tylko w
repozytorium, NIE zastosowana do bazy** (`dotnet ef database update` nie
został uruchomiony, bo to zmiana bazy danych wymaga świadomej decyzji, nie
efekt uboczny refaktoryzacji nazwy pola). Do podjęcia: czy i kiedy zastosować
tę migrację.
