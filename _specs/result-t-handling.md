# Ujednolicenie Obsługi Result&lt;T&gt; w UI

## Cel

Ujednolicić sposób, w jaki Blazorowy UI obsługuje wynik `Result&lt;T&gt;` zwracany przez metody dodawania/edycji/usuwania w warstwie UseCases, tak żeby `Message` zawsze docierał do użytkownika przy błędzie, a nie tylko `Success`/`Fail` bez treści, oraz żeby poprawić istniejący błąd, w którym niepowodzenie operacji pokazuje użytkownikowi komunikat sukcesu.

## Kontekst / stan obecny

- `Result&lt;T&gt;` (`AssetSquirrel.CoreBusiness/Result.cs`) to prosty typ z `Success` (bool), `Message` (string?), `Data` (T?) oraz statycznymi `Ok()`/`Fail()` i mapperem `Select&lt;TOut&gt;`. Jest zwracany przez każdą metodę mutującą (Add/Update/Delete) w `PluginInterfaces`, implementacjach repozytoriów i interfejsach UseCase.
- W UI istnieją dziś **dwa niespójne wzorce** obsługi tego wyniku:
  - **Wzorzec A (częstszy):** komponenty `*AddDialogBox.razor`/`*EditDialogBox.razor` (np. moduły Employees, Locations, Invoices, Dictionares — HardwareType, Suppilers, Manufacturer) wołają use case, wyciągają tylko `.Success` i przekazują dalej goły `bool` przez `EventCallback&lt;bool&gt;`. Strona-rodzic (np. `Employees.razor`) pokazuje wtedy generyczny, zaszyty na sztywno komunikat ("Change saved." / "Saving error.") — realny `Result.Message` z use case'a nigdy nie trafia do użytkownika.
  - **Wzorzec B (rzadszy, lepszy):** moduły Equipment, EquipmentHandover, EquipmentReturn przekazują cały `Result&lt;T&gt;` do rodzica, który przy błędzie woła `JSRuntime.InvokeVoidAsync("OperationAborted", result.Message ?? "Saving error.")` — tu `Message` faktycznie jest widoczny dla użytkownika.
- Powiadomienia w UI działają wyłącznie przez JS-interop do biblioteki iziToast (`wwwroot/iziToast/iziToastReadyFunction.js`, funkcje `OperationSuccessful`, `OperationAborted`, `Information`) wołane ad hoc z każdej strony osobno — nie ma żadnego współdzielonego komponentu/helpera Blazor do jednolitej obsługi wyniku.
- **Zidentyfikowany błąd:** w `Locations/Locations.razor` (metody `ChangeLocationEquipmentStorage`, `OnSaveHideModalEditLocation`, `DeleteLocation` i dodawanie) gałąź niepowodzenia woła `OperationSuccessful("Saving error.")` zamiast `OperationAborted(...)` — użytkownik widzi zielony toast sukcesu, mimo że operacja się nie powiodła, a treść `Result.Message` jest odrzucana już wcześniej, bo handler przyjmuje tylko `bool`.
- Nie każda metoda mutująca w aplikacji stosuje konwencję `Result&lt;T&gt;`: `IFileManagementRepository` (`AddNewFile`, `CreateFolder`, `DeleteFiles`) i `IErrorsRepository.AddErrorAsync` zwracają goły `bool` zamiast `Result&lt;T&gt;`, w przeciwieństwie do repozytoriów encji biznesowych (np. `LocationRepository`), które konsekwentnie opakowują operacje w `Result&lt;T&gt;` z `try/catch` → `Result.Fail(e.Message)`.
- Testy w `AssetSquirrel.UseCases.Tests` konsekwentnie sprawdzają `result.Success` i `result.Message` przy błędach, ale w większości przypadków sukcesu nie sprawdzają `result.Data` na zwróconym `Result&lt;T&gt;` (weryfikują mapowanie pośrednio, przez argument przechwycony w mocku repozytorium) — możliwa luka w pokryciu testami mapowania Dto.

## Zakres (co wchodzi)

- Ujednolicenie sposobu przekazywania wyniku operacji mutującej z dialogów (`*AddDialogBox.razor`/`*EditDialogBox.razor`) do stron-rodziców w całej aplikacji, tak żeby `Result.Message` był zawsze dostępny na końcu łańcucha wywołań, a nie gubiony po drodze jako sam `bool`.
- Naprawa błędu w `Locations.razor`, gdzie niepowodzenie operacji pokazuje toast sukcesu zamiast błędu.
- Ustalenie i zastosowanie jednego wspólnego sposobu (helpera/wzorca) wyświetlania wyniku operacji (sukces/błąd) w oparciu o `Result&lt;T&gt;`, używanego identycznie w każdym module (Employees, Equipment, EquipmentHandover, EquipmentReturn, Invoices, Locations, Dictionares).
- Weryfikacja i ewentualne dopasowanie pozostałych modułów, które dziś pokazują generyczny komunikat zamiast `Result.Message`.

## Poza zakresem (co nie wchodzi)

- Migracja `IFileManagementRepository`/`IErrorsRepository` z `bool` na `Result&lt;T&gt;` — to osobna, większa zmiana konwencji w warstwie repozytoriów; zidentyfikowana tu jako obserwacja, ale wymaga osobnej decyzji/spec.
- Rozbudowa pokrycia testami `UseCases.Tests` o asercje na `result.Data` przy sukcesie — możliwy osobny temat, nie jest przedmiotem tej zmiany UI.
- Zmiana biblioteki powiadomień (iziToast) na inną lub budowa natywnego komponentu Blazor do toastów — zakres ogranicza się do poprawnego przekazywania i wykorzystania istniejącego mechanizmu JS-interop.
- Zmiana logiki biznesowej w UseCases/repozytoriach — `Result&lt;T&gt;` już tam działa poprawnie, problem dotyczy wyłącznie warstwy UI.

## Historyjki użytkownika

- Jako użytkownik aplikacji chcę widzieć konkretny powód niepowodzenia operacji (np. "Nazwa lokalizacji już istnieje"), a nie tylko ogólny komunikat "Saving error." albo — co gorsza — fałszywy komunikat sukcesu.
- Jako użytkownik modułu Lokalizacje chcę, żeby nieudana zmiana/usunięcie/dodanie lokalizacji pokazywało czerwony komunikat błędu, a nie zielony komunikat sukcesu.
- Jako deweloper dodający nowy formularz Add/Edit chcę mieć jeden, oczywisty i powtarzalny sposób obsługi zwróconego `Result&lt;T&gt;`, żeby nie kopiować niespójnych wzorców z różnych modułów.

## Wymagania funkcjonalne

1. Każdy komponent dialogowy (`*AddDialogBox.razor`/`*EditDialogBox.razor`) musi przekazywać do rodzica informację wystarczającą do pokazania `Result.Message` użytkownikowi przy niepowodzeniu — nie tylko `bool`.
2. Każda strona-rodzic obsługująca wynik zapisu/edycji/usunięcia musi przy niepowodzeniu wyświetlić `Result.Message`, jeśli jest dostępny, z sensownym komunikatem zapasowym, gdy `Message` jest puste.
3. Błąd w `Locations.razor` (pokazywanie toastu sukcesu przy niepowodzeniu) musi zostać naprawiony we wszystkich czterech miejscach, w których występuje (dodawanie, edycja, zmiana lokalizacji magazynowej sprzętu, usuwanie).
4. Sposób obsługi wyniku (sprawdzenie `Success`, wybór funkcji toastu, przekazanie `Message`) musi być spójny między wszystkimi modułami — ten sam wzorzec zastosowany identycznie wszędzie, gdzie dziś obsługiwany jest `Result&lt;T&gt;` po zapisie/edycji/usunięciu.
5. Zmiana nie może naruszyć istniejącego zachowania przy sukcesie operacji (dotychczasowe komunikaty sukcesu i odświeżanie list muszą działać tak jak dziś).

## Kryteria sukcesu

- Dla każdego modułu (Employees, Equipment, EquipmentHandover, EquipmentReturn, Invoices, Locations, Dictionares) niepowodzenie operacji Add/Edit/Delete pokazuje użytkownikowi realny `Result.Message` z use case'a, a nie generyczny, zaszyty na sztywno tekst.
- W module Lokalizacje niepowodzenie operacji zawsze pokazuje czerwony komunikat błędu, nigdy zielony komunikat sukcesu.
- Nowy formularz Add/Edit dodany w przyszłości może skorzystać z jednego, udokumentowanego wzorca obsługi `Result&lt;T&gt;`, bez konieczności zgadywania, który z dotychczasowych dwóch wzorców skopiować.
- Żadna z istniejących ścieżek sukcesu nie uległa regresji (te same komunikaty i zachowanie list co przed zmianą).

## Pytania otwarte

1. Czy naprawę błędu w `Locations.razor` (pokazywanie sukcesu przy błędzie) należy potraktować priorytetowo i wdrożyć niezależnie/szybciej niż resztę ujednolicenia, czy ma to być jedna, wspólna zmiana?
2. Czy dopuszczalne jest zmienienie sygnatur istniejących `EventCallback&lt;bool&gt;` na `EventCallback&lt;Result&lt;T&gt;&gt;` (lub podobne) w dialogach, skoro CLAUDE.md wspomina, że te sygnatury "zostały zachowane bez zmian" jako świadoma decyzja — czy to ograniczenie nadal obowiązuje, czy można je teraz zmienić w ramach tej funkcji?
3. Jaki ma być domyślny/zapasowy komunikat błędu, gdy `Result.Message` jest puste (`null`) — zostać przy dotychczasowych tekstach w stylu "Saving error." czy ujednolicić na jeden wspólny tekst?
4. Czy warto przy okazji rozszerzyć zakres o migrację `IFileManagementRepository`/`IErrorsRepository` na `Result&lt;T&gt;`, czy zostaje to świadomie odłożone jako osobny temat (rekomendacja: osobny temat, patrz "Poza zakresem")?

1. Jedna wspólna zmiana
2. Tak - zmodyfikuje CLAUDE.md
3. Błąd zapisu / Błąd aktualizacji / Błąd operacji
4. Rozszeż zmiany.
5. Dopilnuj, by na widokach pojawiał się odpowedni tekst przy wykorzystaniu Jawait JSRuntime.InvokeVoidAsync("OperationSuccessful", "tekst");