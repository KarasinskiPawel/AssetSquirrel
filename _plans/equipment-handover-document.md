# Equipment Handover Document — Implementation Plan

## Context

Moduł "Equipment Handover" (wydanie sprzętu) w AssetSquirrel jest dziś w
praktyce pustym stubem: strona listy (`/equipmenthandover`) nie renderuje
żadnej listy i wyszukiwanie robi tylko `console.log`; formularz tworzenia
(`/addequipmenthandover`) ma pola nagłówkowe (lokalizacja/pracownik
docelowy, komentarz), ale sekcja "Pozycje do przekazania" jest pustym
placeholderem i **nie ma w ogóle przycisku zapisu** — dokumentu wydania nie
da się dziś utworzyć. Spec: `_specs/equipment-handover-document.md`.

Cel: dokończyć funkcję od zera do końca — wybór odbiorcy, przypisanie
konkretnych sztuk sprzętu do dokumentu, zapis do bazy, wygenerowanie
wydruku PDF na firmowym papierze listowym, a po fizycznym podpisaniu —
załączenie skanu podpisanego dokumentu z powrotem do rekordu.

Właściciel produktu odpowiedział na pytania otwarte ze specyfikacji oraz
rozstrzygnął dodatkowe pytania projektowe zadane podczas planowania — te
odpowiedzi są tu traktowane jako wiążące wymagania (patrz każda sekcja
niżej).

## Zweryfikowane fakty o kodzie (potwierdzone odczytem plików)

- `EquipmentHandover.cs`: `HandoverDocumentNumber` (kolumna `nvarchar(12)`,
  `[Required][MaxLength(12)]`), `FromLocationId` (int, **wymagane, nie
  nullable**), `FromLocation`, `ToLocationId` (int?), `ToLocation`,
  `FromEmployeeId` (int?), `FromEmployee`, `ToEmployeeId` (int,
  **wymagane, nie nullable**), `ToEmployee`, `HandoverDate`, `Comment`,
  `IsPosted` (bool), `IsActive` (bool), `EquipmentHandoverDetails`.
  Asymetria `ToLocationId` (nullable) vs `ToEmployeeId` (nie-nullable) jest
  błędem — dziś schemat wymusza pracownika przy każdym wydaniu, mimo że
  spec jawnie wymaga też wydania samej lokalizacji (i wydania jednocześnie
  obu — patrz "Cele" w specu).
- `EquipmentHandoverDetail.cs`: dziś referuje `HardwareTypeId` (typ/kategoria
  sprzętu), nie konkretny egzemplarz — do zmiany na `EquipmentId`.
- `IEquipmentHandoverRepository`/`EquipmentHandoverRepository` (EF): ma już
  pełne CRUD (`Add`/`Update`/`Delete`/`GetEquipmentHandoversAsync`), zwraca
  `Result<EquipmentHandover>`, jeden `CreateDbContext()` na wywołanie —
  wzorzec identyczny jak `EquipmentRepository`.
- `AddEquipmentHandoverUseCase`/`IAddEquipmentHandoverUseCase`: ma dziś
  **tylko** metody odczytu (`GetEquipmentAsync`, `GetLocationsAsync`,
  `GetEmployeesAsync`) — brak jakiejkolwiek metody zapisu.
- `EquipmentAssignment.cs`/`EquipmentAssignmentHistory.cs`: encje + EF
  config + migracje już istnieją i są zaaplikowane do bazy, ale **nie ma
  żadnego repozytorium ani UseCase** — zero logiki biznesowej. Pola:
  `EquipmentId`, `LocationId?`, `EmployeeId?`, `DateOfHandover?`,
  `DateOfReturn?`, `UserId?` (kto wykonał zmianę), plus historia. Brak
  `IsActive` — "aktualnie przypisane" wynika z `DateOfReturn == null`.
- `Equipment.cs` nie ma pola "aktualny posiadacz" — `RegisteredByUserId`
  to jawnie "kto zarejestrował rekord", nie "kto go posiada" (już
  skomentowane w kodzie).
- `ApplicationUser` (konto logowania) **nie ma żadnego powiązania** z
  `Employee` (potwierdzone: `Employee.cs` nie ma `ApplicationUserId`,
  `ApplicationUser.cs` nie ma kolekcji `Employees`) — istotne, bo
  odpowiedź "strona wydająca to zalogowany użytkownik" nie da się
  zamodelować przez `FromEmployeeId`.
- Brak w całym repo jakiejkolwiek biblioteki do generowania PDF (sprawdzone
  we wszystkich 6 plikach `.csproj`) oraz brak jakiejkolwiek prawdziwej
  logiki numerowania dokumentów (numer faktury jest dziś po prostu
  zapisywany tak, jak przyszedł z UI, bez generowania) — obecny placeholder
  `$"{DateTime.Now.Year}\\{DateTime.Now.Month}\\0001"` zawsze zwraca
  literalnie `0001`.
- Wzorzec załączania plików (Invoices), do odtworzenia dla podpisanego PDF:
  `IFileManagementRepository.AddNewFile(id, fileName, contentType, stream)`
  → `FileManagementRepository` (`AssetsSquirrel.Plugins.InMemory`, plik
  `Files/FileManagementRepository.cs`) zapisuje do
  `wwwroot/Files/{Folder}/{id}/{fileName}` (jeden plik na rekord, usuwa
  poprzedni). Encja trzyma tylko `FilePath`/`UploadDate` jako zwykłe kolumny
  (bez osobnej tabeli dokumentów). UI: `<InputFile accept="application/pdf">`
  + `IBrowserFile.OpenReadStream(maxAllowedSize)`. Podgląd/pobranie: JS
  interop `openInNewTab(filePath)` z `wwwroot/Scripts/fileManager.js`
  (funkcja ogólna, gotowa do reużycia bez zmian).
- Firmowy papier listowy już istnieje: `AI-Data/handover/DRUK_FIRMOWY.pdf`
  (branding Sklepy Komfort S.A. — logo w rogu, stopka z NIP/KRS — treść
  środkowa strony pusta, do wypełnienia).

## Decyzje projektowe (rozstrzygnięte z właścicielem produktu)

1. **Strona wydająca = nowe pole `PreparedByUserId`.** Nie próbujemy wiązać
   `Employee` z `ApplicationUser`. `EquipmentHandover` dostaje nowe pole
   `PreparedByUserId` (string?) + nawigację do `ApplicationUser` (dokładnie
   ten sam wzorzec co `Equipment.RegisteredByUserId`) — ustawiane w
   kodzie na ID zalogowanego użytkownika, nigdy nie edytowalne w UI.
   `FromLocationId`/`FromEmployeeId` pozostają w schemacie, ale nie są
   wypełniane ani pokazywane w UI — `FromLocationId` musi zostać zmienione
   na `int?` (dziś wymagane, a nic go nie ustawia).
2. **Numeracja dokumentów: prosty retry przy konflikcie**, nie dedykowana
   tabela licznika z blokadami SQL. Unique index na
   `HandoverDocumentNumber` w bazie jako siatka bezpieczeństwa + ograniczona
   liczba ponownych prób (2–3×) przy konflikcie zapisu. Proporcjonalne do
   realnego ryzyka w małym, wewnętrznym narzędziu — spójne z tym, jak
   niedawno potraktowano duplikaty numeru seryjnego sprzętu w tej samej
   bazie kodu.
3. **`IsPosted` = zaksięgowany, gdy załączono podpisany PDF** (nie w
   momencie pierwszego zapisu dokumentu). Ustawiane przez nową akcję
   "załącz podpisany dokument", nie przez `SaveHandoverAsync`.
4. **Zaksięgowany dokument można całościowo anulować** (nie edytować
   pozycja-po-pozycji). Anulowanie: `IsActive = false` na całym dokumencie
   + automatyczne zamknięcie (`DateOfReturn = now`) wszystkich powiązanych,
   otwartych `EquipmentAssignment` utworzonych przez ten dokument — sprzęt
   wraca do puli dostępnych. Wymaga nowego pola `EquipmentAssignment
   .EquipmentHandoverId` (nullable FK), żeby anulowanie zamykało *dokładnie*
   przypisania utworzone przez ten konkretny dokument, a nie jakiekolwiek
   inne, nowsze przypisanie tego samego sprzętu.
5. **Osobna klasa plikowa dla wydań** (nie parametryzacja istniejącego
   `IFileManagementRepository`) — zero ryzyka dla działającego dziś
   mechanizmu faktur, zgodnie z tym, jak repo już traktuje podobne
   repozytoria (osobna klasa per funkcja, nie współdzielona abstrakcja).

## Zmiany w modelu danych

### `EquipmentHandoverDetail.cs`
- Usunąć `HardwareTypeId`/`HardwareType`.
- Dodać `EquipmentId` (int, wymagane) + `Equipment? Equipment` (nawigacja).
  Producent/typ/model/numer seryjny do tabeli PDF pochodzą tranzytywnie
  przez `Equipment`, bez duplikowania pól.
- `EquipmentHandoverDetailConfiguration.cs`: zamienić
  `HasOne(a => a.HardwareType)` na `HasOne(d => d.Equipment).WithMany()
  .HasForeignKey(d => d.EquipmentId).OnDelete(DeleteBehavior.Restrict)`.
  Dodać unikalny indeks `(EquipmentHandoverId, EquipmentId)` — blokuje
  dodanie tej samej sztuki sprzętu dwa razy do jednego dokumentu.

### `EquipmentHandover.cs`
- `FromLocationId`: `int` → `int?` (nieużywane w UI, patrz decyzja #1).
- `ToEmployeeId`: `int` → `int?` (naprawa asymetrii — dokument musi móc
  celować w samą lokalizację, bez pracownika).
- Dodać `PreparedByUserId` (string?) + `ApplicationUser? PreparedByUser`
  (nawigacja), analogicznie do `Equipment.RegisteredByUserId`.
- Dodać `FilePath` (string?) i `UploadDate` (DateTime?) — dla podpisanego
  skanu, dokładnie jak `Invoice.FilePath`/`UploadDate`.
- `IsPosted`, `IsActive` zostają, z doprecyzowaną semantyką z decyzji #3/#4.
- `EquipmentHandoverConfiguration.cs`: dodać
  `HasOne(e => e.PreparedByUser).WithMany().HasForeignKey(e =>
  e.PreparedByUserId).OnDelete(DeleteBehavior.Restrict)`. Dodać unikalny
  indeks na `HandoverDocumentNumber` (decyzja #2).

### `EquipmentAssignment.cs`
- Dodać `EquipmentHandoverId` (int?) + `EquipmentHandover? EquipmentHandover`
  (nawigacja) — pozwala jednoznacznie powiązać przypisanie z dokumentem,
  który je utworzył (potrzebne do anulowania, decyzja #4).
- Dodać **filtrowany unikalny indeks** w `EquipmentAssignmentConfigurations
  .cs`: `HasIndex(a => a.EquipmentId).IsUnique().HasFilter("[DateOfReturn]
  IS NULL")` — na poziomie bazy gwarantuje, że sprzęt nie może mieć dwóch
  równocześnie otwartych przypisań (twarda ochrona przed podwójnym
  wydaniem tej samej sztuki, niezależnie od tego, co sprawdzi warstwa
  aplikacji).

### DTO (`EquipmentHandoverDto.cs`, `EquipmentHandoverDetailDto.cs`)
- Naprawić istniejący błąd: `EquipmentHandoverDto.EquipmentHandoverDetails`
  jest dziś typu `IEnumerable<EquipmentHandoverDetail>` (encja, nie DTO) —
  zmienić na `List<EquipmentHandoverDetailDto>`, żeby Mapster i Razor
  działały poprawnie.
- `EquipmentHandoverDetailDto`: `HardwareTypeId` → `EquipmentId` +
  spłaszczone pola do wyświetlenia (`ModelName`, `SerialNumber`,
  `ManufacturerName`, `HardwareTypeName`) — ten sam wzorzec co `EquipmentDto`.
- `EquipmentHandoverDto`: dodać `PreparedByUserId`, `PreparedByUserName`,
  `FilePath`, `UploadDate`.

### Migracja
Jedna migracja EF Core łącząca: `HardwareTypeId`→`EquipmentId` na
`EquipmentHandoverDetails` (+ unique index), `FromLocationId`/`ToEmployeeId`
na nullable, nowe kolumny `PreparedByUserId`/`FilePath`/`UploadDate` na
`EquipmentHandovers` (+ unique index na `HandoverDocumentNumber`), nowa
kolumna `EquipmentHandoverId` na `EquipmentAssignments` (+ filtrowany
unique index na `EquipmentId`). Przed napisaniem migracji: `dotnet ef
migrations list`, żeby potwierdzić, że `AddMissingEquipmentAndHandoverColumns`
jest już zaaplikowana i nowa migracja nie koliduje kolejnością.

## UseCase / repozytoria — nowe elementy

Trzymamy się wzorca: jeden folder na obszar funkcjonalny w `UseCases`,
`Result<T>`, `.Adapt<T>()`, ręczne DI w `Extensions/*.cs`.

- **`IEquipmentHandoverRepository`** — dodać `PostEquipmentHandoverAsync
  (EquipmentHandover handover, string preparedByUserId)`: w jednym
  `CreateDbContext()` — (1) wygenerować `HandoverDocumentNumber` (odczyt
  max numeru dla prefiksu `yyyy/MM/`, +1, format `yyyy/MM/nnnn` — dokładnie
  12 znaków), (2) dla każdej pozycji `EquipmentHandoverDetails` utworzyć
  `EquipmentAssignment` (EquipmentId, LocationId=handover.ToLocationId,
  EmployeeId=handover.ToEmployeeId, DateOfHandover=handover.HandoverDate,
  DateOfReturn=null, UserId=preparedByUserId, EquipmentHandoverId=—po
  zapisie handover), (3) dodać snapshot do `EquipmentAssignmentHistories`
  (ten sam wzorzec co `BuildHistorySnapshot` w `EquipmentRepository`),
  (4) jeden `SaveChangesAsync()` na całość. Złapać `DbUpdateException`:
  konflikt unikalności numeru → wygenerować nowy numer i spróbować
  ponownie (max 2-3 próby); konflikt filtrowanego unique indexu na
  `EquipmentAssignments` → `Result.Fail("Jedna lub więcej pozycji zostało
  już przypisanych, odśwież i spróbuj ponownie.")`.
- Dodać `CancelEquipmentHandoverAsync(int equipmentHandoverId, string
  cancelledByUserId)`: ustawia `IsActive=false` na dokumencie, zamyka
  (`DateOfReturn=now` + snapshot historii) wszystkie `EquipmentAssignment`
  z `EquipmentHandoverId == id AND DateOfReturn IS NULL`, jeden
  `SaveChangesAsync()`.
- **Nowe `IEquipmentAssignmentRepository`/`EquipmentAssignmentRepository`**
  — minimalne, tylko odczyt: `GetAssignedEquipmentIdsAsync()` (WHERE
  `DateOfReturn IS NULL`) — używane do filtrowania listy "dostępny sprzęt"
  w formularzu dodawania.
- **`AddEquipmentHandoverUseCase`**: rozszerzyć konstruktor o
  `IEquipmentAssignmentRepository`; `GetEquipmentAsync` filtruje wynik,
  wykluczając ID-ki z `GetAssignedEquipmentIdsAsync()`; dodać
  `Task<Result<EquipmentHandoverDto>> SaveHandoverAsync(EquipmentHandoverDto
  handover, List<int> equipmentIds, string preparedByUserId)` — mapuje DTO
  na encję, buduje `EquipmentHandoverDetail` z `equipmentIds`, woła
  `PostEquipmentHandoverAsync`, mapuje wynik z powrotem.
- Dodać `IEditEquipmentHandoverUseCase` (minimalny — tylko
  `UpdateEquipmentHandover`/`CancelEquipmentHandover`, potrzebny dla
  akcji anulowania i dla ustawienia `FilePath`/`IsPosted` po załączeniu PDF).
- **Nowa para plikowa (decyzja #5)**: `IEquipmentHandoverFileManagementRepository`
  / `EquipmentHandoverFileManagementRepository` (`AssetsSquirrel.Plugins
  .InMemory`) — kopia `FileManagementRepository` z `BaseFolder =
  @"Files\EquipmentHandovers"`.
- **`IAddEquipmentHandoverDocumentUseCase`/`AddEquipmentHandoverDocumentUseCase`**
  — kopia kształtu `AddInvoiceDocumentUseCase`: zapisuje plik przez nowe
  repo plikowe, ustawia `FilePath`/`UploadDate`/`IsPosted=true`, woła
  `IEditEquipmentHandoverUseCase.UpdateEquipmentHandover`.
- **PDF**: `IEquipmentHandoverPdfGenerator` (interfejs w `UseCases`,
  implementacja w `AssetSquirrelAuthorize.WebApp` — potrzebuje dostępu do
  pliku szablonu przez `IWebHostEnvironment.WebRootPath`, więc skopiować
  `DRUK_FIRMOWY.pdf` do `wwwroot/Templates/`). Biblioteka: **PdfSharp**
  (empira, MIT, wsparcie net8.0) — otwiera istniejący PDF
  (`PdfReader.Open(path, PdfDocumentOpenMode.Modify)`) i rysuje na nim
  przez `XGraphics.FromPdfPage(page)`: datę, tabelę pozycji (producent/typ
  sprzętu/model/numer seryjny), kto sporządził/dla kogo, miejsca na
  podpisy. Świadomie **nie** QuestPDF — licencja Community jest darmowa
  tylko do progu przychodu firmy, a to realna spółka handlowa (Sklepy
  Komfort S.A.), więc ryzyko licencyjne jest realne, nie teoretyczne.
  Pakiet dodać tylko do `AssetSquirrelAuthorize.WebApp.csproj`.
- **Nowy endpoint** w `Program.cs`: `GET /api/equipmenthandover/{id}/pdf`
  (minimal API, `RequireAuthorization()`) — generuje i streamuje PDF
  (`Results.File(bytes, "application/pdf")`). Blazor Server nie ma innego
  prostego sposobu na przesłanie wygenerowanych bajtów do nowej karty.
- **DI**: rozszerzyć `EquipmentHandoverExtension.cs` o wszystkie powyższe
  (`services.AddScoped<...>()`, ten sam styl co dziś).

## UI

- **`EquipmentHandover.razor`** (lista): realne ładowanie i renderowanie
  listy (`GetEquipmentHandoverAsync(a => true)`), wyszukiwanie faktycznie
  filtrujące po `HandoverDocumentNumber.Contains(searchText)` (dziś tylko
  `console.log`). Kolumny: numer dokumentu, data, do kogo/gdzie,
  przygotował, status (zaksięgowany/nie, anulowany/nie). Akcje: "Drukuj/
  Podgląd PDF" (`openInNewTab` na nowy endpoint), "Załącz podpisany
  dokument" (nowy dialog, tylko gdy jeszcze nie załączono), "Pobierz
  załącznik" (gdy `FilePath is not null`, ten sam wzorzec co w
  `Invoices.razor`), "Anuluj" (gdy `IsActive`, wywołuje
  `CancelEquipmentHandover`).
- **`AddEquipmentHandover.razor`**: dodać brakujący selektor pozycji pod
  "Pozycje do przekazania" — lista/tabela dostępnego sprzętu (już
  przefiltrowana przez `GetEquipmentAsync` o przypisane sztuki), każda
  pozycja z przyciskiem dodania do roboczej listy `selectedEquipmentIds`.
  Usunąć fałszywy placeholder `HandoverDocumentNumber` — numer jest
  wyłącznie generowany po stronie serwera, nigdy nie pokazywany/edytowalny
  przed zapisem. Dodać brakujący przycisk "Zapisz": walidacja (min. jedna
  pozycja + co najmniej lokalizacja lub pracownik docelowy), pobranie ID
  zalogowanego użytkownika (dokładnie ten sam wzorzec co w
  `EquipmentAddDialogBox.razor`: `AuthenticationStateProvider` +
  `ClaimTypes.NameIdentifier`), wywołanie `SaveHandoverAsync`, nawigacja do
  `/equipmenthandover` przy sukcesie, komunikat `result.Message` przy
  błędzie (analogicznie do niedawnej pracy nad `Equipment`).
  **Aktualizacja (przegląd wizualny po wdrożeniu):** sekcja pozycji ma
  układ dwukolumnowy (`row` + dwa `col-6`) zamiast dwóch tabel jedna pod
  drugą — lewa połowa: "Dostępny sprzęt" z czterema polami filtra
  (Manufacturer/Hardware type/Model/Serial number, filtrowanie lokalne
  po już wczytanej liście, `@bind:event="oninput"`, bez round-tripu do
  serwera), prawa połowa: "Pozycje do przekazania" (bez zmian
  funkcjonalnych, tylko przeniesiona obok). Filtrowana lista wyklucza
  jednocześnie już wybrane pozycje (`FilteredAvailableEquipment()` łączy
  filtr tekstowy z wykluczeniem `selectedEquipmentIds`).
- **Nowy `EquipmentHandoverAddDocumentDialogBox.razor`** — kopia
  `InvoiceAddDocumentDialogBox.razor`: pola tylko do odczytu (numer
  dokumentu, data, odbiorca), `<InputFile accept="application/pdf">`
  (limit 10 MB jak w Invoices), zapis przez
  `AddEquipmentHandoverDocumentUseCase`.
- `/equipmentassignment`, `/equipmentreturn` — bez zmian, poza zakresem.

## Testy

- `AssetSquirrel.UseCases.Tests/EquipmentHandover/`: rozszerzyć
  `AddEquipmentHandoverUseCaseTests` o testy `SaveHandoverAsync` (mapowanie
  DTO→encja, propagacja `Result.Fail` z repozytorium, poprawne wykluczanie
  przypisanego sprzętu w `GetEquipmentAsync` przy zamockowanym
  `IEquipmentAssignmentRepository`). Nowy
  `AddEquipmentHandoverDocumentUseCaseTests.cs` — 1:1 z kształtem
  `AddInvoiceDocumentUseCaseTests.cs` (zapis pliku i delegacja, błąd przy
  nieudanym zapisie, ustawienie `FilePath`/`UploadDate`/`IsPosted` przed
  wywołaniem update use case'a).
- Logika w repozytorium (numeracja, transakcyjny zapis, filtrowany unique
  index) nie ma dziś odpowiednika testów jednostkowych w repo (żadne
  repozytorium EF nie jest dziś testowane jednostkowo — tylko warstwa
  UseCase, z mockowanym repozytorium) — pokryta wyłącznie checklistą
  manualną niżej, zgodnie z istniejącą konwencją testowania tego repo.

## Weryfikacja

1. `dotnet build AssetSquirrel.sln` — 0 błędów (zwrócić uwagę na wszystkie
   miejsca referujące `EquipmentHandoverDetail.HardwareTypeId` — zgrepować
   przed zmianą).
2. `dotnet ef migrations list` / `dotnet ef database update` — nowa
   migracja aplikuje się czysto na istniejącej bazie.
3. `dotnet test AssetSquirrel.UseCases.Tests` — wszystkie testy przechodzą.
4. Uruchomić aplikację, zalogować się:
   - `/equipmenthandover` — lista się renderuje, wyszukiwanie po numerze
     dokumentu faktycznie filtruje.
   - `/addequipmenthandover` — selektor pokazuje tylko dostępny
     (nieprzypisany) sprzęt; wybrać 1-2 pozycje, ustawić lokalizację i/lub
     pracownika, zapisać — sprawdzić w bazie: `EquipmentHandovers` z
     poprawnym numerem (`yyyy/MM/nnnn`, 12 znaków), `EquipmentHandoverDetails`
     z prawidłowymi `EquipmentId`, nowe `EquipmentAssignments` z
     `DateOfReturn IS NULL` i poprawnym `EquipmentHandoverId`.
   - Ponownie otworzyć formularz dodawania — właśnie wydany sprzęt nie
     pojawia się już na liście dostępnych.
   - Na liście: "Drukuj/Podgląd PDF" otwiera nową kartę z PDF-em zgodnym z
     layoutem `DRUK_FIRMOWY.pdf` (data, tabela pozycji, przygotował/dla
     kogo, miejsca na podpisy, bez nachodzenia na logo/stopkę).
   - "Załącz podpisany dokument" — upload działa, `IsPosted` zmienia się
     na `true`, pojawia się przycisk pobrania załącznika.
   - "Anuluj" na zaksięgowanym dokumencie — `IsActive` na `false`,
     powiązane `EquipmentAssignments` zamknięte (`DateOfReturn` ustawione),
     sprzęt wraca na listę dostępnych w nowym formularzu.
5. Potwierdzić, że `/equipmentassignment` i `/equipmentreturn` pozostają
   niezmienione (nadal puste stuby) — brak scope creep.

## Kluczowe pliki

- `AssetSquirrel.CoreBusiness/EquipmentHandover.cs`,
  `EquipmentHandoverDetail.cs`, `EquipmentAssignment.cs`
- `AssetSquirrel.CoreBusiness/Dto/EquipmentHandoverDto.cs`,
  `EquipmentHandoverDetailDto.cs`
- `AssetsSquirrel.Plugins.EfCoreSqlServer/.../EntityConfigurations/
  EquipmentHandoverConfiguration.cs`, `EquipmentHandoverDetailConfiguration.cs`,
  `EquipmentAssignmentConfigurations.cs`
- `AssetsSquirrel.Plugins.EfCoreSqlServer/.../Repositories/
  EquipmentHandoverRepository.cs` (najważniejszy plik — transakcyjny zapis,
  numeracja, przypisania)
- Nowe: `IEquipmentAssignmentRepository`/`EquipmentAssignmentRepository`,
  `IEquipmentHandoverFileManagementRepository`/
  `EquipmentHandoverFileManagementRepository`,
  `IAddEquipmentHandoverDocumentUseCase`/`AddEquipmentHandoverDocumentUseCase`,
  `IEditEquipmentHandoverUseCase`, `IEquipmentHandoverPdfGenerator`
- `AssetSquirrel.UseCases/EquipmentHandover/AddEquipmentHandoverUseCase.cs`
  + `Interfaces/IAddEquipmentHandoverUseCase.cs`
- `AssetSquirrelAuthorize.WebApp/Components/Pages/EquipmentHandover/
  EquipmentHandover.razor`, `AddEquipmentHandover.razor`
- Nowy: `EquipmentHandoverAddDocumentDialogBox.razor`
- `AssetSquirrelAuthorize.WebApp/Extensions/EquipmentHandoverExtension.cs`
- `AssetSquirrelAuthorize.WebApp/Program.cs` (nowy endpoint PDF)
- `AssetSquirrelAuthorize.WebApp/AssetSquirrelAuthorize.WebApp.csproj`
  (referencja do PdfSharp)
- Nowa migracja w `AssetsSquirrel.Plugins.EfCoreSqlServer/.../Migrations/`
