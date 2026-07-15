# Eksport Danych do Excel dla Equipment, Equipment Assignment i Invoices

## Cel

Dodać możliwość eksportu do pliku Excel danych z trzech widoków list: Equipment, Equipment Assignment i Invoices — z uwzględnieniem aktualnie ustawionych filtrów/wyszukiwania, tak żeby użytkownik mógł wyciągnąć z aplikacji tabelaryczny raport do dalszej pracy (np. w Excelu) bez ręcznego kopiowania danych z ekranu.

## Kontekst / stan obecny

- W repozytorium **nie ma dziś żadnej biblioteki do generowania plików Excel** (sprawdzono wszystkie `.csproj` — brak ClosedXML/EPPlus/NPOI/OpenXML SDK) — trzeba dodać nową zależność.
- **Equipment i Equipment Assignment mają dziś wirtualizację przewijania** (`<Virtualize>`, z niedawnej sesji) — widok w przeglądarce w danym momencie ma wczytaną tylko część wyników (partiami po ~200 wierszy), NIE całą przefiltrowaną listę. Eksport "tego, co widać" nie ma więc sensu — eksport musi iść po **całym przefiltrowanym zbiorze po stronie serwera**, tym samym filtrem/wyszukiwaniem, co widok (predykat z `EquipmentRequest`/`EquipmentAssignmentFilter`), a nie po już wczytanych do przeglądarki wierszach.
- Istnieją już gotowe metody zwracające **cały** przefiltrowany zbiór bez stronicowania: `IViewEquipmentUseCase.GetEquipmentAsync(predicate)` i `IViewEquipmentAssignmentUseCase.GetEquipmentAssignmentOverviewAsync(filter)` (używane historycznie przed wprowadzeniem wirtualizacji, wciąż istnieją i działają) — to są naturalne punkty startowe do zasilenia eksportu danymi, ale przy dużych zbiorach (Equipment ma dziś ~14 400 rekordów) wczytanie całości do pamięci serwera na potrzeby eksportu odtwarza dokładnie ten problem pamięciowy, który niedawno rozwiązano wirtualizacją — wymaga to przemyślenia (patrz Pytania otwarte).
- **Invoices** nie ma dziś wirtualizacji/stronicowania — `IViewInvoicesUseCase.GetInvoicesAsync(predicate)` zawsze zwraca całą przefiltrowaną listę, używaną też bezpośrednio przez widok. Skala jest dziś znacznie mniejsza niż Equipment (rząd tysięcy, nie dziesiątek tysięcy).
- Kolumny widoczne dziś na ekranie (kandydaci do kolumn w eksporcie, do potwierdzenia):
  - **Equipment**: Suppiler name, Manufacturer name, Hardware type, Model name, Serial number, Inventory number, Invoice number, Description, Date add, Date removed, Is active?, Added by.
  - **Equipment Assignment**: Location, Person, Handover date, Suppiler, Manufacturer, Type, Model, Serial number, Inventory number, Invoice number, Status (Assigned/Available).
  - **Invoices**: Invoice number, Invoice date, Description, File path, File upload date, User.
- Filtry dziś działające na każdym z trzech widoków, które eksport powinien respektować:
  - Equipment: Suppiler, Manufacturer, Type, Active/Inactive, wyszukiwanie tekstowe (numer seryjny/model/numer inwentarzowy).
  - Equipment Assignment: Location, Employee, Manufacturer, Type, Active/Inactive, wyszukiwanie tekstowe (numer seryjny/inwentarzowy/faktury), sortowanie po Location/Person.
  - Invoices: wyszukiwanie tekstowe (numer faktury/opis) — brak innych filtrów dziś.

## Zakres (co wchodzi)

- Przycisk/akcja "Eksportuj do Excel" na widokach Equipment, Equipment Assignment i Invoices, generująca plik `.xlsx` do pobrania przez użytkownika.
- Eksport uwzględnia aktualnie ustawione na danym widoku filtry i wyszukiwanie tekstowe (eksportowany zbiór odpowiada temu, co użytkownik aktualnie przefiltrował, nie całej nieprzefiltrowanej bazie).
- Kolumny w eksporcie odpowiadające kolumnom widocznym dziś w tabeli na ekranie (patrz lista w sekcji Kontekst) dla każdego z trzech widoków.
- Dodanie do projektu biblioteki do generowania plików `.xlsx` (wybór konkretnej biblioteki — do ustalenia w planie implementacji).

## Poza zakresem (co nie wchodzi)

- Eksport do innych formatów (CSV, PDF) — tylko Excel (`.xlsx`).
- Eksport z innych widoków list w aplikacji (Employees, Locations, EquipmentHandover, EquipmentReturn, Users, widoki słownikowe) — tylko trzy wymienione widoki.
- Formatowanie/stylowanie pliku Excel (kolory, czcionki, formuły) poza podstawowym, czytelnym układem tabeli z nagłówkami kolumn.
- Import danych z Excela (temat odrębny, nie dotyczy tego zadania).
- Zmiana istniejącego mechanizmu filtrowania/wyszukiwania/wirtualizacji na tych widokach — eksport ma z niego korzystać, nie go zmieniać.

## Historyjki użytkownika

- Jako użytkownik przeglądający listę sprzętu (lub przypisań, lub faktur) chcę wyeksportować aktualnie przefiltrowany widok do pliku Excel, żeby dalej pracować z danymi poza aplikacją (np. w raporcie dla przełożonego).
- Jako użytkownik, który ustawił konkretne filtry (np. tylko sprzęt danego producenta), chcę, żeby eksport zawierał wyłącznie te przefiltrowane dane, a nie wszystko, co jest w bazie.

## Wymagania funkcjonalne

1. Na każdym z trzech widoków (Equipment, Equipment Assignment, Invoices) musi być widoczna akcja umożliwiająca eksport aktualnie wyświetlanych (przefiltrowanych) danych do pliku `.xlsx`.
2. Eksport musi odpytywać serwer o **cały** przefiltrowany zbiór (nie tylko wiersze aktualnie wczytane w przeglądarce przez wirtualizację), stosując te same kryteria filtrowania/wyszukiwania, co bieżący stan widoku w momencie kliknięcia eksportu.
3. Wygenerowany plik musi mieć nagłówki kolumn odpowiadające kolumnom widocznym na ekranie dla danego widoku (patrz lista w sekcji Kontekst) oraz zawierać wszystkie wiersze spełniające bieżący filtr.
4. Plik musi się pobrać do przeglądarki użytkownika (standardowe zachowanie pobierania pliku), z sensowną nazwą pliku wskazującą, z którego widoku pochodzi (np. zawierającą nazwę widoku i datę wygenerowania).
5. Generowanie eksportu nie może destabilizować/zawieszać aplikacji przy dużych zbiorach (Equipment ~14 400 rekordów) — sposób pobierania danych do eksportu musi być zaprojektowany z uwzględnieniem tego ryzyka (patrz Pytania otwarte #2).

## Kryteria sukcesu

- Kliknięcie eksportu na każdym z trzech widoków pobiera plik `.xlsx` zawierający dokładnie te wiersze, które spełniają aktualnie ustawiony filtr/wyszukiwanie na danym widoku.
- Nagłówki kolumn w pliku są czytelne i odpowiadają kolumnom z widoku ekranowego.
- Eksport dużego, nieprzefiltrowanego zbioru (Equipment, ~14 400 rekordów) kończy się sukcesem, bez zawieszenia serwera/przeglądarki i w rozsądnym czasie.
- Brak regresji w istniejącym filtrowaniu, wyszukiwaniu, sortowaniu i wirtualizacji na trzech widokach.

## Pytania otwarte

1. Czy eksport ma dotyczyć wyłącznie kolumn widocznych dziś na ekranie, czy powinien zawierać też dodatkowe pola, które są w danych, ale nie są dziś wyświetlane w tabeli (np. identyfikatory, pełne daty z godziną)?
2. Przy dużych zbiorach (Equipment ~14 400 rekordów) — czy eksport ma być generowany od razu, synchronicznie, na żądanie (użytkownik czeka na pobranie pliku), czy potrzebny jest jakiś mechanizm w tle (np. przygotowanie pliku i powiadomienie/link do pobrania), żeby nie blokować requestu na dłuższy czas?
3. Czy przycisk eksportu ma być widoczny dla każdego zalogowanego użytkownika, czy tylko dla administratorów (`AdminOnly`), tak jak część innych akcji na tych widokach?
4. Czy na Equipment Assignment eksport powinien też respektować aktualnie ustawione sortowanie (Location/Person), czy kolejność wierszy w pliku Excel nie ma znaczenia?
5. Czy nazwa/lokalizacja generowanego pliku ma jakieś konkretne wymagania (np. konwencja nazewnictwa firmy), czy wystarczy nazwa typu `equipment-2026-07-15.xlsx`?


## Odpowiedzi

1. Kolumny widoczne na ekranie
2. Wykonaj asynchronicznie z jakimś mały powiadomieniem w prawym górnym rogu ekranu, że trwa przygotowanie pliku.
3. Dla każdego zalogowanego użytkownika.
4. Bez znaczenia.
5. "2026-07-15-equipment.xlsx"