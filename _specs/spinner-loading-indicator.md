# Spinner (Loading Indicator) na Widokach Listy Danych

## Cel

Dodać widoczny wskaźnik ładowania (spinner) na każdym widoku listy danych w aplikacji, wyświetlany podczas pobierania danych z bazy, tak żeby użytkownik od razu widział, że strona się wczytuje, a nie patrzył na pustą tabelę do momentu, aż dane się pojawią.

## Kontekst / stan obecny

- **Żaden z widoków listy w aplikacji nie pokazuje dziś żadnego wskaźnika ładowania.** Każda strona pobiera dane w `OnInitializedAsync` i renderuje tabelę Bootstrap; przed zakończeniem pobierania `<tbody>` jest po prostu pusty (brak wierszy) — na `Invoices.razor` i `EquipmentAssignment.razor` w tym czasie migawkowo widać nawet komunikat "brak danych"/"no data", mimo że dane wciąż się ładują, nie że ich nie ma.
- Widoki objęte tym zakresem (wszystkie pobierają listę w `OnInitializedAsync` i renderują tabelę): `Equipment.razor`, `Employees.razor`, `Locations.razor`, `Invoices.razor`, `EquipmentHandover.razor`, `EquipmentReturn.razor`, `EquipmentAssignment.razor` (`Components/Pages/EquipmetAssignment/`), `Users.razor`, oraz trzy strony słownikowe: `DictionaryHardwareType.razor`, `DictionaryManufacturer.razor`, `DictionarySuppilers.razor` (pod `Components/Pages/Dictionares/`). `DictionaryEquipment.razor`, `DictionaryInternet.razor` i `Hardware.razor` to dziś tylko zaślepki (`<h3>`, brak pobierania danych) — nie dotyczy ich ten zakres.
- **Istnieje już bliźniaczy, ale nieużyty poza dwoma miejscami wzorzec:** `EquipmentAddDialogBox.razor` i `EquipmentEditDialogBox.razor` mają pole `bool isLoading = true` i blok `@if (isLoading) { <div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div> } else { ...formularz... }`, ustawiane na `false` po zakończeniu pobrania danych do rozwijanych list w `OnInitializedAsync`. To jedyne miejsca w kodzie z jakimkolwiek spinnerem — wzorzec jest skopiowany ręcznie między tymi dwoma plikami, nie wydzielony do współdzielonego komponentu.
- **Nie istnieje żaden współdzielony komponent spinnera/loadera.** `Components/Template/` zawiera tylko `AdminGuard.cs`, `AdminOnly.razor`, `DialogBox.razor`, `TextSearchBar.razor` — żaden z nich nie obsługuje stanu ładowania.
- **Brak stylowania spinnera pod ciemny/jasny motyw.** Bootstrapowy `.spinner-border` dziedziczy kolor z `currentColor` (kolor tekstu otoczenia) — żadny z plików `theme-tokens.css`/`theme-components.css`/`app.css` nie nadpisuje tego, w przeciwieństwie do większości innych elementów UI (przyciski, inputy), które mają dedykowane, motyw-zależne style (`--as-accent-cyan` itd.).
- **Wszystkie widoki listy używają `@rendermode InteractiveServer`** (nie statycznego SSR) — `OnInitializedAsync` odpala się raz podczas prerenderowania i drugi raz po podłączeniu obwodu interaktywnego (SignalR). To oznacza, że naiwna implementacja może dać efekt "mignięcia" (spinner znika i wraca) przy przejściu z prerenderowania do trybu interaktywnego — trzeba to uwzględnić w implementacji, żeby spinner nie migał bez potrzeby.

## Zakres (co wchodzi)

- Nowy, wspólny reużywalny komponent spinnera (np. `Components/Template/LoadingSpinner.razor`) — jeden standard wizualny, użyty na wszystkich 10 wymienionych widokach listy, widoczny od momentu wejścia na stronę do momentu, aż wszystkie zapytania startowe danej strony są zakończone.
- Zamiana dzisiejszego ręcznie skopiowanego markupu `isLoading`/`spinner-border` w `EquipmentAddDialogBox.razor` i `EquipmentEditDialogBox.razor` na ten sam nowy wspólny komponent (ujednolicenie — jeden standard wszędzie, zgodnie z Odpowiedzią #1).
- Większy, bardziej dopasowany wizualnie spinner z podpisem tekstowym, z osobnym stylem dla motywu light i dla dark (nie jeden uniwersalny kolor).

## Poza zakresem (co nie wchodzi)

- Strony-zaślepki bez pobierania danych (`DictionaryEquipment.razor`, `DictionaryInternet.razor`, `Hardware.razor`) — nie dotyczy ich ten zakres, bo nie mają czego "ładować".
- Wskaźniki ładowania przy akcjach innych niż początkowe wczytanie listy (np. zapis formularza, wyszukiwanie/filtrowanie po już wczytanej liście, usuwanie wiersza) — chyba że odpowiedzi na pytania otwarte to rozszerzą.
- Zmiana samego mechanizmu pobierania danych (np. paginacja, wirtualizacja, lazy loading) — to tylko wizualny wskaźnik podczas istniejącego sposobu pobierania całej listy na raz.

## Historyjki użytkownika

- Jako użytkownik otwierający listę sprzętu/pracowników/lokalizacji itd. chcę widzieć, że strona się wczytuje, a nie patrzeć na pustą tabelę, zastanawiając się, czy coś jest nie tak.
- Jako użytkownik na wolniejszym połączeniu chcę mieć jasny sygnał wizualny, że dane są w drodze, zamiast migawkowego komunikatu "brak danych", który sugeruje, że lista jest naprawdę pusta.

## Wymagania funkcjonalne

1. Każdy z 10 wymienionych widoków listy musi pokazywać wskaźnik ładowania od momentu wejścia na stronę do momentu, aż odpowiadające mu dane są wczytane i gotowe do wyrenderowania.
2. Wskaźnik ładowania musi zniknąć i ustąpić miejsca normalnej tabeli/listy natychmiast po zakończeniu pobierania danych.
3. Wskaźnik ładowania musi zawsze odzwierciedlać rzeczywisty stan pobierania danych w danym cyklu życia komponentu — bez dodatkowego mechanizmu zapobiegającego ponownemu pojawieniu się przy przejściu z prerenderowania statycznego do trybu `InteractiveServer` (świadomie zaakceptowane, patrz Odpowiedzi #5); to nie jest traktowane jako błąd, tylko jako naturalny efekt tego, że `OnInitializedAsync` faktycznie odpala się drugi raz.
4. Wskaźnik ładowania musi być czytelny w obu motywach (light/dark), z kolorem dopasowanym do pozostałych elementów UI (nie goły domyślny Bootstrap).
5. Istniejące zachowanie stron dla przypadku "brak danych" (np. "No invoices found.", "No equipment matches the current filters.") musi pojawiać się tylko PO zakończeniu ładowania, gdy lista jest faktycznie pusta — nie w trakcie samego ładowania.

## Kryteria sukcesu

- Wejście na każdy z 10 widoków listy pokazuje wspólny komponent spinnera, który ustępuje miejsca danym, gdy wszystkie zapytania startowe danej strony się zakończą.
- Widoki z komunikatem "brak danych" nie pokazują go już migawkowo podczas ładowania — tylko spinner, a potem albo dane, albo faktyczny komunikat o braku danych.
- Wygląd spinnera jest identyczny (ten sam komponent) na wszystkich widokach i wyraźnie różny między motywem light i dark, z czytelnym podpisem tekstowym.
- `EquipmentAddDialogBox.razor`/`EquipmentEditDialogBox.razor` używają tego samego nowego komponentu zamiast własnego, ręcznie skopiowanego markupu, bez regresji w ich dzisiejszym zachowaniu.

## Pytania otwarte

1. Czy wspólny, reużywalny komponent spinnera (do wydzielenia z istniejącego markupu w dialogach Equipment) jest w zakresie tego zadania, czy wystarczy powtórzyć ten sam markup na każdym z 10 widoków bez wydzielania komponentu?
2. Jak dokładnie powinien wyglądać spinner — dokładnie ten sam `spinner-border` co w dialogach Equipment (mały, wyśrodkowany w kontenerze o `min-height: 200px`), czy inny rozmiar/układ bardziej pasujący do pełnowymiarowej strony listy (np. na środku całej zawartości, z podpisem tekstowym typu "Ładowanie...")?
3. Czy przy stronach z wieloma niezależnymi zapytaniami startowymi (np. `EquipmentAssignment.razor` — 4 listy słownikowe + główna lista, dziś pobierane sekwencyjnie jedna po drugiej) spinner ma się pokazywać do zakończenia WSZYSTKICH zapytań, czy tylko głównej listy danych (a filtry/dropdowny mogą się doładować w tle)?
4. Czy dotyczy to też strony `Users.razor` (lista kont z rolami/blokadami, w zakresie admina), mimo że nie została wymieniona w oryginalnym poleceniu wprost jako "widok" — traktuję ją jako listę danych analogiczną do innych, ale proszę o potwierdzenie.
5. Czy przy nawrocie z prerenderowania do `InteractiveServer` (opisane w kontekście) wystarczy, że spinner po prostu zostaje widoczny przez cały ten czas (nawet jeśli oznacza to, że pierwsze show trwa bardzo krótko a potem ponownie ładuje), czy oczekujesz jakiegoś dodatkowego mechanizmu, żeby uniknąć powtórnego pobierania danych przy drugim uruchomieniu `OnInitializedAsync`?

## Odpowiedzi

1. **Wspólny reużywalny komponent — jeden standard wszędzie.** Jeden komponent (np. `Components/Template/LoadingSpinner.razor`) użyty na wszystkich 10 widokach listy, i docelowo także jako zamiennik dzisiejszego ręcznie skopiowanego markupu w `EquipmentAddDialogBox.razor`/`EquipmentEditDialogBox.razor` — nie osobne, powtórzone bloki `@if (isLoading) { ... }` w każdym pliku.
2. **Duży, dopasowany osobno dla jasnego i ciemnego motywu, z tekstem.** Nie mały `spinner-border` wzorowany 1:1 na dialogach — większy, bardziej widoczny wskaźnik, z podpisem tekstowym (np. "Ładowanie..."), i z **osobnym** stylem/kolorystyką dla motywu light i dla dark (nie jeden uniwersalny kolor działający "jakoś" w obu).
3. **Do zakończenia wszystkich zapytań startowych.** Na stronach z wieloma niezależnymi zapytaniami przy starcie (np. `EquipmentAssignment.razor` — 4 listy słownikowe + główna lista) spinner zostaje widoczny, aż WSZYSTKIE się zakończą, nie tylko główna lista danych.
4. **Tak.** `Users.razor` wchodzi do zakresu jako pełnoprawny widok listy, tak jak pozostałe 9.
5. **Bez dodatkowych mechanizmów.** Nie wprowadzamy nic specjalnego, żeby zapobiec drugiemu odpaleniu `OnInitializedAsync` przy przejściu z prerenderowania do `InteractiveServer` (np. żadnego flagowania "już pobrane" między trybami) — spinner po prostu odzwierciedla aktualny stan pobierania w danym cyklu życia komponentu, nawet jeśli to znaczy, że przy tym przejściu na chwilę pojawi się ponownie. To zmienia Wymaganie funkcjonalne #3 (patrz niżej) — nie wymuszamy braku tego zjawiska, tylko godzimy się z nim jako świadomym, zaakceptowanym kompromisem.
