# Brak obsługi wyjątków i logowania błędów w EquipmentHandoverFileManagementRepository

## Problem

`EquipmentHandoverFileManagementRepository.AddNewFile` (oraz pozostałe metody tej klasy) wykonuje operacje na systemie plików (`Directory.CreateDirectory`, `File.Create`, `Stream.CopyToAsync`, `Directory.GetFiles`) bez żadnego bloku `try/catch`, w przeciwieństwie do każdego repozytorium EF Core w tym samym obszarze (`EquipmentHandoverRepository`, `EquipmentRepository` itd.), które konsekwentnie łapią wyjątki, logują je przez `IErrorsRepository.AddErrorAsync` i zwracają wynik błędu zamiast pozwolić wyjątkowi propagować się dalej.

## Lokalizacja

- `AssetsSquirrel.Plugins/AssetsSquirrel.Plugins.InMemory/AssetsSquirrel.Plugins.InMemory/Files/EquipmentHandoverFileManagementRepository.cs:60-81` (metoda `AddNewFile`)
- Kontrast z konwencją: `AssetsSquirrel.Plugins/AssetsSquirrel.Plugins.EfCoreSqlServer/AssetsSquirrel.Plugins.EFCoreSqlServer/Repositories/EquipmentHandoverRepository.cs:46-67` (każda metoda mutująca ma `try/catch` + `errorsRepository.AddErrorAsync(...)`)
- Wywołanie bez zabezpieczenia po stronie use case: `AssetSquirrel.UseCases/EquipmentHandover/AddEquipmentHandoverDocumentUseCase.cs:22-34`

## Wpływ

Jeśli zapis pliku się nie powiedzie z przyczyn środowiskowych (brak miejsca na dysku, brak uprawnień do `wwwroot/Files/EquipmentHandovers`, zbyt długa ścieżka, równoczesny dostęp blokujący plik), wyjątek (`IOException`, `UnauthorizedAccessException` itp.) propaguje się przez `AddEquipmentHandoverDocumentUseCase.AddEquipmentHandoverDocumentAsync` w górę do komponentu `EquipmentHandoverAddDocumentDialogBox.razor`, który nie ma żadnego `try/catch` wokół wywołania use case'u. W Blazor Server nieobsłużony wyjątek w metodzie obsługi zdarzenia powoduje zerwanie obwodu SignalR (użytkownik widzi "There was an unhandled exception..." i musi odświeżyć całą stronę), zamiast czytelnego komunikatu błędu w istniejącym mechanizmie `JSRuntime.InvokeVoidAsync("OperationAborted", ...)`, który jest już używany w tym samym komponencie dla innych ścieżek błędu (`AddEquipmentHandoverDocumentDialogBox.razor:97`, "No file selected!").

## Proponowana naprawa

1. Owinąć logikę I/O w `EquipmentHandoverFileManagementRepository.AddNewFile` w `try/catch`, przechwytując wyjątki i zwracając `false` (zgodnie z obecną sygnaturą `Task<bool>`) zamiast pozwalać na propagację.
2. Wstrzyknąć `IErrorsRepository` do konstruktora (analogicznie do repozytoriów EF Core) i wywołać `errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentHandoverFileManagementRepository", "AddNewFile", ex)` w bloku `catch`, żeby błąd trafił do tego samego centralnego logu co pozostałe awarie repozytoriów.
3. Zaktualizować rejestrację w `AssetSquirrelAuthorize.WebApp/Extensions/EquipmentHandoverExtension.cs` — konstruktor z dodatkową zależnością `IErrorsRepository` zostanie rozwiązany automatycznie przez DI, o ile `IErrorsRepository` jest już zarejestrowane globalnie (do potwierdzenia w `Program.cs`/odpowiednim rozszerzeniu).
4. Rozważyć zastosowanie tej samej poprawki w analogicznym `FileManagementRepository` dla faktur (poza ścisłym zakresem tego planu, ale ten sam defekt tam występuje) — najlepiej razem z refaktoryzacją opisaną w `zduplikowana-logika-zarzadzania-plikami.md`, żeby poprawka błędu i redukcja duplikacji poszły w jednej zmianie.

## Weryfikacja

- Dodać test jednostkowy (jeśli po refaktoryzacji z `zduplikowana-logika-zarzadzania-plikow.md` powstanie testowalna abstrakcja nad systemem plików) weryfikujący, że wyjątek rzucony podczas zapisu pliku skutkuje zwróceniem `false`, a nie propagacją wyjątku.
- Manualnie: tymczasowo ustawić folder `wwwroot/Files/EquipmentHandovers` jako tylko do odczytu (lub wskazać nieistniejącą, niezapisywalną ścieżkę) i potwierdzić, że próba dodania podpisanego dokumentu w UI kończy się komunikatem błędu (`OperationAborted`) zamiast zerwania obwodu Blazor.
- `dotnet build AssetSquirrel.sln` bez błędów po dodaniu zależności `IErrorsRepository`.
