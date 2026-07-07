# Zduplikowana logika przechowywania plików między EquipmentHandover a Invoices

## Problem

`EquipmentHandoverFileManagementRepository` jest niemal bajt w bajt kopią `FileManagementRepository` (używanego przez faktury) — te same metody (`IfFolderExist`, `CreateFolder`, `IfFilesExist`, `DeleteFiles`, `AddNewFile`), ta sama logika, różni się tylko nazwą parametru (`equipmentHandoverId` vs `invoiceId`) i stałą folderu bazowego. Jest to duplikacja logiki, którą łatwo uogólnić na jedną implementację parametryzowaną nazwą podfolderu.

## Lokalizacja

- `AssetsSquirrel.Plugins/AssetsSquirrel.Plugins.InMemory/AssetsSquirrel.Plugins.InMemory/Files/EquipmentHandoverFileManagementRepository.cs:1-84`
- `AssetsSquirrel.Plugins/AssetsSquirrel.Plugins.InMemory/AssetsSquirrel.Plugins.InMemory/Files/FileManagementRepository.cs:1-84`
- Interfejsy z identyczną sygnaturą: `AssetSquirrel.UseCases/PluginInterfaces/IEquipmentHandoverFileManagementRepository.cs`, `AssetSquirrel.UseCases/PluginInterfaces/IFileManagementRepository.cs`

## Wpływ

- Każda poprawka błędu lub zmiana zachowania (np. dodanie obsługi wyjątków, zmiana strategii nazewnictwa plików, dodanie limitu rozmiaru) musi być ręcznie powielona w dwóch miejscach; łatwo o rozjazd zachowania między obszarami (już teraz obie kopie nie obsługują wyjątków — patrz osobny plan `brak-obslugi-bledow-w-repozytorium-plikow.md`).
- Zwiększa czas przeglądu kodu i ryzyko, że przyszła poprawka trafi tylko do jednej z dwóch kopii.

## Proponowana naprawa

1. Wprowadzić jedną klasę bazową/generyczną, np. `LocalDiskFileManagementRepository` w `AssetsSquirrel.Plugins.InMemory/Files`, przyjmującą nazwę podfolderu (`"Invoices"` / `"EquipmentHandovers"`) przez konstruktor lub parametr generyczny, implementującą wspólną logikę raz.
2. `FileManagementRepository` i `EquipmentHandoverFileManagementRepository` stają się cienkimi klasami dziedziczącymi po wspólnej bazie i wskazującymi właściwy interfejs (`IFileManagementRepository` / `IEquipmentHandoverFileManagementRepository`), albo — jeśli interfejsy są identyczne strukturalnie — rozważyć ich scalenie w jeden generyczny interfejs przyjmujący identyfikator encji i nazwę obszaru (wymaga jednak zmiany rejestracji DI w `EquipmentHandoverExtension.cs` i analogicznej klasie dla faktur — do wykonania ostrożnie, żeby nie naruszyć istniejących rejestracji).
3. Zachować rejestrację DI w dotychczasowych, per-funkcjonalnych klasach `Extensions/*.cs` (`EquipmentHandoverExtension.AddExtension`, odpowiednik dla faktur) — zmienia się tylko implementacja pod interfejsem, nie miejsce rejestracji.

## Weryfikacja

- `dotnet build AssetSquirrel.sln` bez błędów po refaktoryzacji.
- Ręczny test regresyjny: dodanie podpisanego dokumentu do wydania sprzętu (`EquipmentHandoverAddDocumentDialogBox`) oraz dodanie załącznika do faktury (`InvoiceAddDocumentDialogBox`) — oba scenariusze powinny nadal zapisywać plik pod właściwą ścieżką (`wwwroot/Files/EquipmentHandovers/{id}/...` i `wwwroot/Files/Invoices/{id}/...`).
- Jeśli dla wspólnej klasy bazowej powstaną testy jednostkowe (obecnie żaden z plików nie ma testów, ponieważ operują na systemie plików) — rozważyć wstrzyknięcie abstrakcji nad `System.IO` (np. `IFileSystem` z `System.IO.Abstractions`), żeby umożliwić pokrycie testami xUnit bez dotykania rzeczywistego dysku; poza minimalnym zakresem tego planu, ale warto odnotować jako naturalny kolejny krok.
