# Ręczne mapowanie encja->DTO zamiast Mapster.Adapt w ViewEquipmentHandoverUseCase

## Problem

`ViewEquipmentHandoverUseCase.GetEquipmentHandoverAsync` buduje `EquipmentHandoverDto` ręcznie przez inicjalizator obiektu, pole po polu, zamiast użyć `.Adapt<EquipmentHandoverDto>()` z Mapster, jak robią to analogiczne metody `Get*Async` w innych obszarach (`ViewSuppilersUseCase.GetSuppilersAsync`, `ViewManufacturerUseCase`). Ręczne przepisywanie pól jest źródłem błędów przy rozroście encji/DTO — już teraz mapowanie jest niekompletne/niespójne: `ToLocation`/`ToEmployee` są ustawiane wprost, ale analogiczne `FromLocation`/`FromEmployee` (mimo że DTO ma takie właściwości) nie są mapowane w ogóle.

## Lokalizacja

- `AssetSquirrel.UseCases/EquipmentHandover/ViewEquipmentHandoverUseCase.cs:30-60`
- Dla porównania wzorzec z Mapster: `AssetSquirrel.UseCases/Suppilers/ViewSuppilersUseCase.cs:24-27`, `AssetSquirrel.UseCases/Manufacturers/ViewManufacturerUseCase.cs`

## Wpływ

- Każda nowa właściwość dodana do `EquipmentHandover`/`EquipmentHandoverDto` (albo do `EquipmentHandoverDetail`/`EquipmentHandoverDetailDto`) wymaga pamiętania o ręcznej aktualizacji tej metody; pominięcie (jak już miało miejsce z `FromLocation`/`FromEmployee`) powoduje ciche, trudne do wykrycia braki danych w UI zamiast błędu kompilacji lub jawnego mapowania.
- Utrzymanie kosztowniejsze niż w reszcie kodu: deweloperzy muszą pamiętać o innej konwencji mapowania tylko w tym jednym use case, co zwiększa ryzyko przy code review i onboardingu.
- Istniejący test `ViewEquipmentHandoverUseCaseTests.GetEquipmentHandoverAsync_ReturnsMappedDtos` weryfikuje tylko wybrane pola (`EquipmentHandoverId`, `HandoverDocumentNumber`, `ToEmployeeId`), więc nie wykryłby regresji w pozostałych polach.

## Proponowana naprawa

1. Dla płaskich pól `EquipmentHandoverDto` (te, które mają dokładnie takie same nazwy jak w encji: `EquipmentHandoverId`, `HandoverDocumentNumber`, `FromLocationId`, `ToLocationId`, `FromEmployeeId`, `ToEmployeeId`, `HandoverDate`, `Comment`, `IsPosted`, `IsActive`, `FilePath`, `UploadDate`) — zastąpić inicjalizację obiektu wywołaniem `h.Adapt<EquipmentHandoverDto>()`, tak jak w `ViewSuppilersUseCase`/`ViewManufacturerUseCase`.
2. Dla pól wymagających spłaszczenia zagnieżdżonych właściwości (`PreparedByUserName` z `PreparedByUser.UserName`, oraz `EquipmentHandoverDetails[].ModelName/SerialNumber/ManufacturerName/HardwareTypeName` z `Equipment`/`Equipment.Manufacturer`/`Equipment.HardwareType`) — ponieważ nazwy DTO nie odpowiadają domyślnej konwencji spłaszczania Mapster (`Equipment.ModelName` -> `ModelName`, a nie `EquipmentModelName`), dodać jawną konfigurację `TypeAdapterConfig` (np. w statycznej klasie `MapsterConfig` rejestrowanej raz w kompozycji, lub lokalnie przez `.ForType<EquipmentHandoverDetail, EquipmentHandoverDetailDto>().Map(...)`), żeby cała logika mapowania (łącznie ze spłaszczeniem) była zadeklarowana w jednym miejscu i objęta przez `.Adapt<T>()`, a nie rozproszona w kodzie use case'u.
3. Po wprowadzeniu jawnej konfiguracji, `GetEquipmentHandoverAsync` powinno sprowadzać się do `return handovers.Adapt<List<EquipmentHandoverDto>>();` (ewentualnie `.ToList()` jeśli `handovers` to `IEnumerable`).
4. Utrzymać zachowanie, że `FromLocation`/`FromEmployee` pozostają niewypełnione w DTO (zgodnie z komentarzem w `EquipmentHandover.cs:16-17`, strona wydająca jest reprezentowana przez `PreparedByUserId`, a nie te pola) — jeśli po analizie okaże się, że pola te są całkowicie martwe, rozważyć ich usunięcie w osobnym PR (poza zakresem tego planu).

## Weryfikacja

- Rozszerzyć `ViewEquipmentHandoverUseCaseTests.GetEquipmentHandoverAsync_ReturnsMappedDtos` o asercje na `PreparedByUserName`, `FilePath`, `UploadDate`, `Comment` oraz na zawartość `EquipmentHandoverDetails` (w tym `ModelName`, `SerialNumber`, `ManufacturerName`, `HardwareTypeName`), żeby przyszła regresja w konfiguracji Mapster była wykrywana.
- Uruchomić `dotnet test AssetSquirrel.UseCases.Tests/AssetSquirrel.UseCases.Tests.csproj` i `dotnet build AssetSquirrel.sln` po zmianie.
