# Brak zabezpieczenia przed przekazaniem null jako preparedByUserId/cancelledByUserId

## Problem

`AddEquipmentHandover.razor` i `EquipmentHandover.razor` odczytują identyfikator zalogowanego użytkownika jako `string?` (`FindFirst(ClaimTypes.NameIdentifier)?.Value`) i przekazują go bez żadnego sprawdzenia do parametrów `preparedByUserId`/`cancelledByUserId`, zadeklarowanych jako nie-nullowalne `string` w interfejsach use case'ów. Kompilator zgłasza to jako ostrzeżenie CS8604 (możliwy null), a strony `Equipment*.razor` w tym samym repo stosują jawne zabezpieczenie (`if (user.Identity?.IsAuthenticated == true)`), którego tu brakuje.

## Lokalizacja

- `AssetSquirrelAuthorize.WebApp/Components/Pages/EquipmentHandover/AddEquipmentHandover.razor:258-261` (`userId` przekazywany do `SaveHandoverAsync`)
- `AssetSquirrelAuthorize.WebApp/Components/Pages/EquipmentHandover/EquipmentHandover.razor:206-210` (`userId` przekazywany do `CancelEquipmentHandoverAsync`)
- Sygnatury nie-nullowalne: `AssetSquirrel.UseCases/EquipmentHandover/Interfaces/IAddEquipmentHandoverUseCase.cs:12`, `AssetSquirrel.UseCases/EquipmentHandover/Interfaces/IEditEquipmentHandoverUseCase.cs:9`
- Potwierdzone przez build: `dotnet build AssetSquirrel.sln` zgłasza CS8604 dla obu wywołań (linie 261 i 210 w plikach `.razor` odpowiednio w `AddEquipmentHandover.razor` i `EquipmentHandover.razor`).
- Kontrast ze wzorcem zabezpieczenia: `AssetSquirrelAuthorize.WebApp/Components/Pages/Equipment/Equipment.razor:209-212`, `EquipmentAddDialogBox.razor:247-250`.

## Wpływ

Jeśli w chwili zapisu/anulowania wydania sprzętu w `ClaimsPrincipal` zabraknie roszczenia `ClaimTypes.NameIdentifier` (np. z powodu niestandardowej konfiguracji Identity, wygaśnięcia/odświeżenia sesji w trakcie długo trwającego obwodu Blazor Server, testów z niestandardowym `AuthenticationStateProvider`), `userId` będzie `null`. Ponieważ nie ma żadnej walidacji przed wywołaniem use case'u:
- `EquipmentHandover.PreparedByUserId` zostanie zapisane jako `null` w bazie (pole jest nullable, więc SaveChanges się powiedzie) — dokument wydania sprzętu trwale traci informację, kto go wystawił, mimo że aplikacja zakłada, że to pole "jest polem faktycznie wypełnianym/wyświetlanym" (patrz komentarz w `EquipmentHandover.cs:33-35`).
- Analogicznie `CancelEquipmentHandoverAsync` zapisze `EquipmentAssignmentHistory.UserId = null`, tracąc informację, kto anulował wydanie — osłabia to ślad audytowy, na którym opiera się cała funkcjonalność historii przypisań.
- Błąd jest cichy: operacja zwraca `Result.Success = true`, użytkownik dostaje komunikat "zapisano pomyślnie", mimo że dane audytowe są niekompletne.

## Proponowana naprawa

1. W `AddEquipmentHandover.razor` i `EquipmentHandover.razor`, przed wywołaniem `SaveHandoverAsync`/`CancelEquipmentHandoverAsync`, dodać jawne sprawdzenie analogiczne do wzorca z `Equipment.razor`:
   ```csharp
   var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
   if (string.IsNullOrEmpty(userId))
   {
       await JSRuntime.InvokeVoidAsync("OperationAborted", "Nie udało się zidentyfikować zalogowanego użytkownika. Zaloguj się ponownie.");
       return;
   }
   ```
2. Dzięki temu do use case'u trafia zawsze `string` niepusty, sygnatury `IAddEquipmentHandoverUseCase.SaveHandoverAsync`/`IEditEquipmentHandoverUseCase.CancelEquipmentHandoverAsync` mogą pozostać nie-nullowalne bez generowania ostrzeżeń kompilatora (CS8604 zniknie).
3. Rozważyć dodanie tego samego zabezpieczenia w miejscu wywołania `AddEquipmentHandoverDocumentUseCase`, jeśli w przyszłości zacznie przyjmować identyfikator użytkownika (obecnie go nie przyjmuje, mimo że ustawia `IsPosted = true` — do weryfikacji, czy to zamierzone, poza zakresem tego planu).

## Weryfikacja

- Po zmianie: `dotnet build AssetSquirrel.sln` nie powinien już zgłaszać ostrzeżeń CS8604 dla `AddEquipmentHandover.razor:261` ani `EquipmentHandover.razor:210`.
- Ręczny/manualny test: zasymulować `IAuthenticationStateProvider` zwracający `ClaimsPrincipal` bez `ClaimTypes.NameIdentifier` (lub odpiąć plik cookie w trakcie sesji) i potwierdzić, że UI pokazuje komunikat błędu zamiast zapisać rekord z pustym `PreparedByUserId`.
