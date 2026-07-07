# Brak testów dla EditEquipmentHandoverUseCase

## Problem

`EditEquipmentHandoverUseCase` (metody `UpdateEquipmentHandoverAsync` i `CancelEquipmentHandoverAsync`) nie ma odpowiadającego pliku testów w `AssetSquirrel.UseCases.Tests`, mimo że pozostałe klasy UseCase w tym samym folderze (`AddEquipmentHandoverUseCase`, `AddEquipmentHandoverDocumentUseCase`, `ViewEquipmentHandoverUseCase`) są pokryte testami zgodnie z konwencją opisaną w `CLAUDE.md`.

## Lokalizacja

- `AssetSquirrel.UseCases/EquipmentHandover/EditEquipmentHandoverUseCase.cs:1-32`
- Brakujący plik: `AssetSquirrel.UseCases.Tests/EquipmentHandover/EditEquipmentHandoverUseCaseTests.cs` (nie istnieje)

Dla porównania istniejące testy w tym samym folderze:
- `AssetSquirrel.UseCases.Tests/EquipmentHandover/ViewEquipmentHandoverUseCaseTests.cs`
- `AssetSquirrel.UseCases.Tests/EquipmentHandover/AddEquipmentHandoverUseCaseTests.cs`
- `AssetSquirrel.UseCases.Tests/EquipmentHandover/AddEquipmentHandoverDocumentUseCaseTests.cs`

## Wpływ

`EditEquipmentHandoverUseCase` obsługuje dwie operacje o realnym wpływie na dane biznesowe: aktualizację dokumentu wydania (używaną także pośrednio przez `AddEquipmentHandoverDocumentUseCase`) oraz anulowanie wydania sprzętu (`CancelEquipmentHandoverAsync`, wywoływane z `EquipmentHandover.razor`). Bez testów jednostkowych:
- regresja w mapowaniu `EquipmentHandoverDto` -> `EquipmentHandover` (np. przez zmianę pola w encji/DTO) nie zostanie wykryta przez CI,
- błędna propagacja `Result<T>.Fail(...)` z repozytorium (np. utrata komunikatu błędu) nie zostanie wykryta,
- `AddEquipmentHandoverDocumentUseCaseTests` mockuje `IEditEquipmentHandoverUseCase` bezpośrednio, więc nie weryfikuje w ogóle rzeczywistej logiki `EditEquipmentHandoverUseCase` — luka jest całkowita, nie tylko częściowa.

## Proponowana naprawa

Dodać `AssetSquirrel.UseCases.Tests/EquipmentHandover/EditEquipmentHandoverUseCaseTests.cs` z testami analogicznymi do istniejącego wzorca (mock `IEquipmentHandoverRepository` z Moq), pokrywającymi:
1. `UpdateEquipmentHandoverAsync` — sukces: weryfikacja, że `handover.Adapt<EquipmentHandover>()` trafia do repozytorium i że wynik repozytorium jest zmapowany z powrotem na `EquipmentHandoverDto` (`result.Select(e => e.Adapt<EquipmentHandoverDto>())`).
2. `UpdateEquipmentHandoverAsync` — propagacja `Result<T>.Fail(...)` gdy repozytorium zwróci błąd.
3. `CancelEquipmentHandoverAsync` — sukces: weryfikacja, że `equipmentHandoverId` i `cancelledByUserId` są przekazywane do repozytorium bez zmian oraz że wynik jest mapowany na DTO.
4. `CancelEquipmentHandoverAsync` — propagacja błędu z repozytorium (np. "Equipment handover not found.").

## Weryfikacja

Uruchomić `dotnet test AssetSquirrel.UseCases.Tests/AssetSquirrel.UseCases.Tests.csproj` i potwierdzić, że nowe testy przechodzą oraz że liczba testów w projekcie wzrosła (obecnie 71 testów przechodzi pomyślnie).
