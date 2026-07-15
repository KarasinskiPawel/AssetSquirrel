# Optymalizacja Pamięci na Widokach Equipment i Equipment Assignment

## Cel

Ograniczyć zużycie pamięci serwera przez widoki `Equipment.razor` i `EquipmentAssignment.razor`, które dziś przy każdym wejściu i przy każdej zmianie filtra/wyszukiwania pobierają i materializują w pamięci **całą** listę wyników (bez podziału na strony), co przy rosnącej liczbie rekordów sprzętu (dziś ~14 400 aktywnych/nieaktywnych pozycji po imporcie historycznym) generuje duży ślad pamięciowy na obwód (circuit) Blazor Server, pomnożony przez liczbę równoczesnych użytkowników.

## Kontekst / stan obecny

- Aplikacja to **Blazor Server** (`@rendermode InteractiveServer`), nie WebAssembly — cały stan strony (w tym pobrane listy danych) żyje w pamięci procesu serwera przez cały czas trwania obwodu (circuit) danego użytkownika, nie w pamięci przeglądarki. Im więcej równoczesnych użytkowników ma otwarty ten widok, tym bardziej mnoży się to zużycie pamięci na serwerze.
- `Equipment.razor` (`Components/Pages/Equipment/Equipment.razor`): `OnInitializedAsync` i `SearchEquipments()` wywołują `IViewEquipmentUseCase.GetEquipmentAsync(predicate)`, które w `EquipmentRepository.GetEquipmentAsync` (warstwa EF Core) robi `dbContext.Equipments.Where(where).Select(...).ToListAsync()` **bez** `Skip`/`Take` — zawsze zwraca **wszystkie** rekordy spełniające filtr, w jednym zapytaniu, i przechowuje je w polu `equipments` (`List<EquipmentDto>`) na cały czas trwania obwodu.
- `EquipmentAssignment.razor` (`Components/Pages/EquipmetAssignment/`): analogicznie, `IViewEquipmentAssignmentUseCase.GetEquipmentAssignmentOverviewAsync(filter)` w warstwie EF Core robi `LEFT JOIN` `Equipments`/`EquipmentAssignments` i `ToListAsync()` na całym wyniku, bez podziału na strony — trzymane w polu `items`.
- Obie strony renderują wynik przez zwykłe `@foreach` do jednej długiej tabeli HTML (`<tbody>`) — przy tysiącach wierszy oznacza to też duże drzewo renderowania Blazor po stronie serwera i potencjalnie ciężkie diff'y SignalR przy każdym odświeżeniu (np. po zmianie filtra).
- Każda zmiana filtra/wyszukiwania (`@bind-Value:after="Search"`/`SearchCallback`) odpytuje bazę od nowa o **cały** zbiór wynikowy, nie o jedną "stronę" — więcej filtrów (Equipment ma dziś 4 filtry + wyszukiwanie tekstowe, Equipment Assignment 5) nie zmniejsza tego problemu, bo użytkownik często zostawia filtry szerokie (np. tylko "Active").
- Żaden inny widok listy w aplikacji (Employees, Locations, Invoices, Handover, Return, Users, słowniki) nie ma dziś paginacji/wirtualizacji — ale to Equipment i Equipment Assignment mają rząd wielkości więcej danych niż pozostałe (dziesiątki tysięcy vs. dziesiątki/setki wierszy), więc są tu w zakresie jako pierwsze.
- Nie istnieje dziś żaden mechanizm stronicowania (paging) po stronie bazy/repozytorium/UI, ani użycie wbudowanego komponentu Blazor `<Virtualize>` gdziekolwiek w aplikacji.

## Zakres (co wchodzi)

- Rozwiązanie ograniczające ilość danych ładowanych i przechowywanych jednocześnie w pamięci serwera dla widoków `Equipment.razor` i `EquipmentAssignment.razor`, zgodne z idiomatycznym podejściem dla Blazor Server (np. stronicowanie po stronie zapytania do bazy i/lub wirtualizacja renderowania z dociąganiem danych na żądanie) — konkretne podejście do wyboru w planie implementacji.
- Zachowanie pełnej funkcjonalności istniejących filtrów i wyszukiwania tekstowego na obu widokach, działających na całym zbiorze danych (nie tylko na już wczytanej "stronie").
- Zachowanie istniejącego sortowania po kolumnach Location/Person na Equipment Assignment.

## Poza zakresem (co nie wchodzi)

- Pozostałe widoki list w aplikacji (Employees, Locations, Invoices, EquipmentHandover, EquipmentReturn, Users, widoki słownikowe) — mają dziś rząd wielkości mniej danych, nie są objęte tym zadaniem.
- Zmiana struktury bazy danych, indeksów czy modelu danych Equipment/EquipmentAssignment.
- Zmiana wyglądu/układu paska filtrów (świeżo poprawionego) — tylko sposób ładowania i przechowywania wyników w pamięci.
- Optymalizacja pamięciowa dialogów Add/Edit Equipment (osobny temat, inny wzorzec ładowania danych).

## Historyjki użytkownika

- Jako administrator infrastruktury chcę, żeby otwarcie widoku Equipment lub Equipment Assignment przez wielu użytkowników jednocześnie nie powodowało nadmiernego zużycia pamięci na serwerze produkcyjnym.
- Jako użytkownik przeglądający listę tysięcy sztuk sprzętu chcę, żeby strona nadal działała płynnie (bez długiego ładowania całej listy na raz) niezależnie od tego, jak duża jest baza sprzętu.
- Jako użytkownik chcę, żeby filtrowanie i wyszukiwanie wciąż działały po całym zbiorze danych, a nie tylko po fragmencie, który akurat jest wczytany na stronie.

## Wymagania funkcjonalne

1. Widoki Equipment i Equipment Assignment nie mogą jednocześnie przechowywać w pamięci obwodu (circuit) całego zbioru wynikowego, gdy ten zbiór jest duży — rozwiązanie musi ograniczać liczbę rekordów trzymanych w pamięci na raz do rozsądnej, ograniczonej wielkości (np. jednej "strony"/widocznego okna wyników), niezależnie od tego, ile wierszy spełnia bieżący filtr.
2. Zmiana filtra lub wpisanie tekstu w wyszukiwarce musi wciąż zwracać poprawny wynik odpowiadający całemu zbiorowi danych spełniającemu kryteria — nie tylko danym już wczytanym do pamięci.
3. Sortowanie po kolumnach Location/Person na Equipment Assignment musi nadal działać poprawnie względem całego przefiltrowanego zbioru, nie tylko wczytanej części.
4. Rozwiązanie nie może wymagać jednorazowego wczytania całej listy do przeglądarki/klienta (to nie jest aplikacja WebAssembly) — ograniczenie zużycia pamięci musi dotyczyć pamięci procesu serwera (obwodu Blazor Server), zgodnie z architekturą tej aplikacji.
5. Istniejące operacje na widoku (dodawanie/edycja/dezaktywacja sprzętu, odświeżenie listy po zapisie w dialogu) muszą nadal działać poprawnie po wprowadzeniu zmiany.

## Kryteria sukcesu

- Otwarcie widoku Equipment lub Equipment Assignment przy dużej liczbie rekordów (rząd ~14 000+) nie powoduje wczytania i przechowania w pamięci serwera całego zbioru na raz.
- Filtrowanie, wyszukiwanie i sortowanie dają identyczne rezultaty jak dziś (przed zmianą), tylko przy niższym zużyciu pamięci.
- Zużycie pamięci na jeden aktywny obwód (circuit) mający otwarty jeden z tych widoków jest wyraźnie i wymiernie niższe niż dziś przy pełnym, nieprzefiltrowanym zbiorze danych.
- Brak regresji w istniejących operacjach (dodawanie, edycja, dezaktywacja, odświeżenie po zapisie) na obu widokach.

## Pytania otwarte

1. Czy preferowane jest klasyczne stronicowanie z widocznymi kontrolkami "poprzednia/następna strona"/numerami stron, czy wirtualizacja przewijania (komponent Blazor `<Virtualize>` z dociąganiem danych na żądanie w miarę przewijania), czy kombinacja obu?
2. Jaki rozmiar "strony"/okna wyników jest akceptowalny z punktu widzenia użyteczności (np. 25, 50, 100 wierszy na raz)?
3. Czy sortowanie po kolumnach Location/Person na Equipment Assignment powinno przejść na sortowanie po stronie zapytania do bazy (ORDER BY w SQL), czy wystarczy, że działa poprawnie w ramach nowego mechanizmu stronicowania/wirtualizacji bez dalszych wymagań co do miejsca wykonania sortowania?
4. Czy ten sam mechanizm ma być od razu zaprojektowany jako reużywalny wzorzec do zastosowania w przyszłości na innych widokach list, czy na razie ograniczamy się wyłącznie do Equipment i Equipment Assignment?
5. Czy istnieje docelowy budżet pamięciowy na serwer (np. maksymalna liczba równoczesnych użytkowników × maksymalne zużycie pamięci na obwód), który powinien być punktem odniesienia przy ocenie, czy rozwiązanie jest "wystarczająco dobre"?

## Odpowiedzi

1. Spróbujmy z wirtualizacją przewijania.
2. 200
3. Wystarczy poprawne działanie w ramach nowego mechanizmu stronicowania
4. Tak
5. Nie - po prostu sensowne gospodarowanie zasobami.