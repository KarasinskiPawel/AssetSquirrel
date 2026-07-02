# Uwagi z przeglądu projektu — status

Notatka z analizy architektury AssetSquirrel. Punkty uporządkowane wg priorytetu.
Legenda: ✅ zrobione / ⏭️ świadomie pominięte (decyzja użytkownika) / 🔲 do zrobienia.

## Zamknięte

- ✅ **Potwierdzanie konta e-mail** — podłączony realny SMTP (`SmtpEmailSender`,
  `mail.markety.komfort.pl:587`), usunięty `IdentityNoOpEmailSender` i scaffoldowy
  bypass w `RegisterConfirmation.razor`. Dane logowania w `dotnet user-secrets`,
  nie w repo. Konfiguracja hosta/portu w `appsettings.json` (`Smtp` section).
- ⏭️ **Anonimizacja seed data pracowników** (`AssetsSquirrelContext.cs`) —
  odrzucone: dane muszą pozostać takie jak są, potrzebne do testów. Nie ruszać.
- ✅ **Połykanie wyjątków bez informacji zwrotnej** — dodany `Result<T>`
  (`AssetSquirrel.CoreBusiness/Result.cs`: `Success`, `Message`, `Data` +
  `.Select()` do mapowania encja→DTO z zachowaniem statusu). Zamienione
  wszystkie metody mutujące (`Add/Update/Delete...Async`), które zwracały
  gołego `bool`, na `Task<Result<T>>` — w interfejsach `PluginInterfaces`,
  implementacjach repozytoriów w `AssetsSquirrel.Plugins.EFCoreSqlServer`
  oraz w interfejsach/implementacjach UseCase'ów, we wszystkich 8 obszarach
  (Employees, Equipment, Invoices, Locations, Manufacturers, Suppilers,
  HardwareType, EquipmentHandover). Metody `Get*Async` (odczyt) zostały
  celowo bez zmian — zwracają listę/DTO jak dotychczas.
  UI (Razor) pozostało przy `EventCallback<bool>` — na wywołaniach
  odpakowywane jest tylko `.Success`, bez wyświetlania `.Message` w UI
  (świadoma decyzja: backend-only na razie, podłączenie do iziToast to
  osobny follow-up jeśli będzie potrzebny). Testy Employees zaktualizowane
  pod nowy typ zwracany. Build całego rozwiązania: 0 błędów, 7/7 testów.
  Przy okazji odkryto, że `IEquipmentHandoverRepository.Add/Update/Delete...Async`
  nie są wywoływane przez żaden UseCase ani UI — funkcja dodawania/edycji/
  usuwania handoveru wygląda na niedokończoną (tylko odczyt jest używany).
  Nie naprawiano — poza zakresem tego zadania, ale warto to zbadać osobno.
- ✅ **`GenericMapper<T,U>` (AutoMapper) zastąpiony Mapsterem** — usunięty
  pakiet `AutoMapper` z `AssetSquirrel.UseCases.csproj` (miał zresztą znaną
  podatność, ostrzeżenie NU1903 przy każdym buildzie), dodany `Mapster`
  10.0.10. Usunięte pliki `AssetSquirrel.UseCases/Mapper/GenericMapper.cs`
  i `IGenericMapper.cs` (martwy kod po migracji). Wszystkie ~60 wywołań
  `new GenericMapper<T,U>().Map(x)` w 8 obszarach zamienione na
  `x.Adapt<T>()` (dla kolekcji: `.Adapt<List<T>>()` zamiast
  `Map(...).ToList()`/`.ContinueWith(...Select...)`). Usunięty też
  nieużywany `using AutoMapper.QueryableExtensions;` w `ISuppilersRepository.cs`
  oraz osierocone `using AssetSquirrel.UseCases.Mapper;` w
  `LocationUseCaseExtensions.cs`, `Locations.razor`, `LocationAddDialogBox.razor`.
  Build całego rozwiązania: 0 błędów, testy: 7/7 (Mapster mapuje po nazwach
  właściwości tak samo jak wcześniejsza konwencja AutoMappera — zero zmian
  w zachowaniu mapowania).
- ✅ **Błędny namespace `IManufacturersRepository`** — zmieniony z
  `AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories` na poprawny
  `AssetSquirrel.UseCases.PluginInterfaces` (zgodnie z resztą interfejsów
  w tym folderze). Zaktualizowane `using`i w 5 plikach, które korzystały
  z tego interfejsu przez stary (przypadkowo działający) namespace:
  `Manufacturers/ViewManufacturerUseCase.cs`, `EditManufactureruseCase.cs`,
  `AddManufacturerUserCase.cs`, `EquipmentUseCase/AddEquipmentUseCase.cs`,
  `EditEquipmentUseCase.cs`. Implementacja (`ManufacturersRepository.cs`
  w pluginie EFCoreSqlServer) i rejestracja DI (`DictionaresUseCaseExtensions.cs`)
  nie wymagały zmian — miały już poprawne `using`i do obu namespace'ów.
  Zweryfikowane: `AssetSquirrel.UseCases.csproj` nadal nie ma referencji
  projektowej do pluginu EFCoreSqlServer — ukryte sprzężenie faktycznie
  usunięte, nie tylko zamaskowane. Build: 0 błędów, testy: 7/7.
- ✅ **Testy jednostkowe + CI** — dodany projekt `AssetSquirrel.UseCases.Tests`
  (xUnit + Moq), dołączony do `.sln`. Dodany workflow
  `.github/workflows/build-and-test.yml` — `dotnet build` + `dotnet test`
  na push/PR do `master` (ubuntu-latest).
- ✅ **Rozszerzone pokrycie testami na wszystkie 8 obszarów** — wzorzec
  z `Employees` (mock repozytorium przez Moq, weryfikacja mapowania
  DTO↔encja, `Result<T>.Success`/`.Message` dla mutacji, weryfikacja
  `Get*` przez porównanie zmapowanej listy) zreplikowany na `Equipment`,
  `Invoices`, `Locations`, `Manufacturers`, `Suppilers`, `HardwareType`,
  `EquipmentHandover` — równolegle przez 7 subagentów, każdy z pełnym
  kontekstem bieżącego kodu produkcyjnego danego obszaru. Po scaleniu
  poprawione 2 kolizje nazw (namespace testowy `...Tests.Equipment`/
  `...Tests.HardwareType` przesłaniał encje `AssetSquirrel.CoreBusiness.
  Equipment`/`HardwareType` w plikach spoza własnego folderu — naprawione
  przez pełne kwalifikowanie `CoreBusiness.Equipment`/`CoreBusiness.
  HardwareType` w 3 miejscach). Build całego rozwiązania: 0 błędów.
  Testy: **64/64 zielone**.
- ✅ **Martwy kod usunięty** — `git rm -r`:
  - `AssetSquirrel.WebApp/` — cały porzucony projekt (2161 śledzonych plików,
    m.in. zasoby wwwroot). Nie był w `.sln`, więc usunięcie nie dotyka builda.
  - Osierocone stuby na płytszym poziomie obu pluginów: `AssetsSquirrel.Plugins.
    EfCoreSqlServer.csproj` + `Class1.cs`, `AssetsSquirrel.Plugins.InMemory.csproj`
    + `Class1.cs`.
  - `AssetSquirrel.CoreBusiness/User/UserCredential.cs` (pusta klasa-zaślepka;
    folder `User/` też usunięty, był pusty po usunięciu pliku).
  Zweryfikowane: `.sln` nie odwoływał się do żadnego z usuniętych plików.
  Build całego rozwiązania: 0 błędów. Testy: 64/64 zielone.

## Do zrobienia

### ⏭️ Niespójne nazewnictwo w repo (świadomie pominięte)
`AssetSquirrel`/`AssetsSquirrel`, `Suppiler` zam. `Supplier`, `Dictionares`
zam. `Dictionaries`, literówki w nazwach klas UseCase typu
`AddManufacturerUserCase` — celowo NIE poprawiane bez wyraźnej prośby (zbyt
duży, ryzykowny rename dotykający m.in. nazw tabel/migracji EF). Trzymać się
istniejącej konwencji w nowym kodzie.

## Zamknięte (drobne porządki)

- ✅ **`IFileManagementRepository` przeniesiony** z
  `AssetsSquirrel.Plugins.InMemory/Files/Interfaces/` do
  `AssetSquirrel.UseCases/PluginInterfaces/` (namespace
  `AssetSquirrel.UseCases.PluginInterfaces`), zgodnie z konwencją reszty
  repozytoriów. Odwrócona zależność projektowa: usunięta referencja
  `AssetSquirrel.UseCases → AssetsSquirrel.Plugins.InMemory` (niepotrzebna
  po przeniesieniu interfejsu), dodana `AssetsSquirrel.Plugins.InMemory →
  AssetSquirrel.UseCases` (plugin implementuje interfejs z warstwy UseCases —
  ten sam wzorzec DIP co dla EFCoreSqlServer). Zaktualizowane `using`i w
  `FileManagementRepository.cs`, `AddInvoiceDocumentUseCase.cs`,
  `FilesRepositoryExtensions.cs` i teście `AddInvoiceDocumentUseCaseTests.cs`.
- ✅ **`global.json` dodany** — przypina SDK do `8.0.422` z
  `rollForward: latestFeature`, zapobiega przypadkowemu użyciu innej
  zainstalowanej wersji SDK (repo miało też 5.0/6.0/7.0/9.0 obok 8.0).
- ✅ **`README.md` uzupełniony** — opis projektu, stos technologiczny, diagram
  warstw, wymagania, kroki uruchomienia lokalnego (connection string,
  migracje, `dotnet run`), komendy build/test.
- ✅ **`CLAUDE.md` odświeżony** — usunięte nieaktualne zapisy (brak testów/CI,
  brak `global.json`, GenericMapper/AutoMapper, brak `Result<T>`,
  `IdentityNoOpEmailSender`, błędny namespace `IManufacturersRepository`,
  `AssetSquirrel.WebApp` jako "historyczne odniesienie" — teraz usunięty),
  dodane sekcje o `Result<T>`, testach, `SmtpEmailSender`, przeniesionym
  `IFileManagementRepository`. Plugin `InMemory` opisany wprost jako
  "nie jest in-memory store" w sekcji architektury (nazwy samego projektu/
  folderu/namespace nie zmieniano — to byłby rename tej samej skali co
  poprawianie `Suppiler`, poza zakresem "drobnych porządków").
  Build całego rozwiązania: 0 błędów. Testy: 64/64 zielone.
