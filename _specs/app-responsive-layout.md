# Responsywny Wygląd Aplikacji przy Zmniejszaniu Okna

## Cel

Dostosować wygląd aplikacji (przede wszystkim szerokie tabele na widokach list oraz paski filtrów) tak, żeby przy zmniejszaniu okna przeglądarki nie dochodziło do "wylewania się" treści poza widoczny obszar strony — jak na zrzucie ekranu (`AI-Data/data/dostosowanie-wygladu.JPG`), gdzie tabela Equipment Assignment jest szersza niż okno, a przewijanie w poziomie dotyczy całej strony, nie samej tabeli.

## Kontekst / stan obecny

- **Sidebar/nawigacja** (`Components/Layout/MainLayout.razor.css`, `NavMenu.razor.css`): ma już jeden punkt przełamania w `641px` — pod tą szerokością boczne menu chowa się za przyciskiem hamburgera (`.navbar-toggler`), powyżej jest sztywny, przypięty sidebar `250px`. To zachowanie ze standardowego szablonu Blazor, obecne od początku projektu, dziś niezmieniane.
- **Tabele na widokach list** (`wwwroot/table-color.css`, klasy `#table-container`/`#table-color`, używane na Equipment, Equipment Assignment, Employees, Locations, Invoices, Handover, Return, Users, widoki słownikowe): `#table-container` ma dziś tylko `max-height: 36rem; overflow-y: auto;` — **wyłącznie przewijanie w pionie**. Nie ma żadnej reguły `overflow-x`, `min-width` czy zapytania `@media` ograniczającego/kompaktującego kolumny przy węższym oknie. Czcionka (`font-size: 12px`) i padding komórek są sztywne, bez zmiany przy mniejszych szerokościach.
- **Tabele z wieloma kolumnami są najbardziej narażone** — Equipment (13 kolumn po połączeniu Serial/Inventory), Equipment Assignment (11 kolumn) — przy typowej szerokości laptopa (~1366px) i mniejszej tabela nie mieści się w oknie, a przeglądarka pokazuje poziomy scrollbar dla całej strony (nie tylko dla `#table-container`), bo nic nie ogranicza szerokości/nie wymusza przewijania lokalnie.
- **Paski filtrów**: w tej sesji (`claude/feature/equipment-filters-toolbar-fix`) dodano `flex-wrap` do pasków filtrów na Equipment i Equipment Assignment (żeby kontrolki zawijały się do nowego wiersza, a nie wystawały poza okno) — ale pozostałe widoki list (Employees, Locations, Invoices, EquipmentHandover, EquipmentReturn, widoki słownikowe) mają dziś prostszy pasek (przycisk Add + wyszukiwanie) bez `flex-wrap`; nie było też celowego testu tych zawijających się pasków przy bardzo wąskich oknach.
- **Widoki Equipment i Equipment Assignment mają teraz wirtualizację przewijania** (`<Virtualize>`, z tej sesji) — każda zmiana układu/szerokości tabeli na tych dwóch widokach musi zachować poprawne działanie wirtualizacji (nie chowa się/nie zawiesza przy zmianie rozmiaru kontenera).
- Reszta layoutu (`app.css`, `theme-tokens.css`) nie ma dodatkowych reguł `@media` poza tymi dwiema wymienionymi wyżej w plikach layoutu.

## Zakres (co wchodzi)

- Kontener tabel na widokach list (`#table-container`) dostaje przewijanie w poziomie ograniczone do samej tabeli (nie do całej strony) na węższych szerokościach okna, tak żeby reszta layoutu (sidebar, górny pasek, przyciski nad tabelą) zostawała na miejscu i nie "rozjeżdżała się".
- Paski filtrów na wszystkich widokach list (nie tylko Equipment/Equipment Assignment) zawijają się poprawnie przy węższym oknie, bez wychodzenia elementów poza widoczny obszar.
- Zachowanie/ewentualne dopracowanie istniejącego punktu przełamania sidebar/hamburger (`641px`), jeśli okaże się niewystarczające w praktyce.

## Poza zakresem (co nie wchodzi)

- Pełny redesign wizualny aplikacji (kolory, typografia, motyw) — temat dotyczy wyłącznie układu/responsywności, nie estetyki.
- Osobny, dedykowany widok/układ mobilny (np. karty zamiast tabeli na telefonie) — jeśli to potrzebne, to odrębny temat.
- Zmiana zestawu kolumn w tabelach (np. chowanie mniej istotnych kolumn na małych ekranach) — chyba że odpowiedzi na pytania otwarte to rozszerzą.
- Zmiana mechanizmu wirtualizacji/stronicowania na Equipment i Equipment Assignment (zostaje, ma tylko dalej poprawnie działać po zmianie CSS).

## Historyjki użytkownika

- Jako użytkownik pracujący na laptopie z mniejszym ekranem chcę widzieć całą stronę (sidebar, przyciski, filtry) bez poziomego przewijania całego okna — tylko sama tabela z danymi powinna przewijać się w poziomie, jeśli nie mieści się w dostępnej szerokości.
- Jako użytkownik zmniejszający okno przeglądarki (np. żeby ustawić je obok innej aplikacji) chcę, żeby pasek filtrów nad tabelą nadal był czytelny i użyteczny, a nie wychodził poza widoczny obszar.

## Wymagania funkcjonalne

1. Na każdym widoku listy (Equipment, Equipment Assignment, Employees, Locations, Invoices, EquipmentHandover, EquipmentReturn, Users, widoki słownikowe) kontener tabeli musi przewijać się w poziomie samodzielnie, gdy szerokość okna jest mniejsza niż naturalna szerokość tabeli — bez wywoływania poziomego przewijania całej strony.
2. Sidebar, górny pasek i przyciski nad tabelą (Add, filtry, wyszukiwanie) muszą pozostawać w pełni widoczne i użyteczne niezależnie od szerokości okna, powyżej i poniżej istniejącego punktu przełamania `641px`.
3. Paski filtrów na wszystkich widokach list muszą zawijać swoje kontrolki do nowego wiersza przy węższym oknie, tak żeby żaden element (dropdown, przycisk, pole wyszukiwania) nie wychodził poza widoczny obszar ani nie nakładał się na inne elementy.
4. Zmiana CSS nie może psuć działania wirtualizacji przewijania na Equipment i Equipment Assignment (`<Virtualize>` musi nadal poprawnie mierzyć wysokość/szerokość kontenera i dociągać dane przy scrollowaniu).
5. Rozwiązanie musi działać spójnie w obu motywach (light/dark), zgodnie z istniejącym mechanizmem tokenów (`theme-tokens.css`).

## Kryteria sukcesu

- Przy zmniejszaniu okna przeglądarki (od pełnego ekranu do szerokości typowego laptopa i węziej) żadna strona listy nie pokazuje poziomego scrollbara dla całej strony — tylko, jeśli trzeba, lokalnie dla samej tabeli.
- Sidebar, górny pasek i pasek filtrów pozostają czytelne i klikalne na każdej testowanej szerokości okna, bez elementów wychodzących poza widoczny obszar.
- Equipment i Equipment Assignment nadal poprawnie przewijają dane (wirtualizacja) po zmianach CSS.
- Wygląd pozostaje spójny w motywie light i dark.

## Pytania otwarte

1. Jaka jest najmniejsza szerokość okna, dla której ma to zadziałać dobrze — typowy laptop (~1366px), tablet w orientacji poziomej (~1024px), czy węziej (np. pół ekranu na monitorze 1920px, czyli ~960px)? Czy telefony (poniżej ~640px, gdzie sidebar już się chowa) są w zakresie tego zadania, czy to osobny temat?
2. Czy przy bardzo wąskim oknie akceptowalne jest poziome przewijanie samej tabeli (użytkownik przewija w bok, żeby zobaczyć wszystkie kolumny), czy oczekujesz czegoś więcej — np. zmniejszenia czcionki/paddingu w komórkach przy węższym oknie, żeby więcej kolumn było widocznych bez przewijania?
3. Czy pasek filtrów na widokach, które go jeszcze nie mają zawijanego (Employees, Locations, Invoices, EquipmentHandover, EquipmentReturn, widoki słownikowe), powinien dostać dokładnie ten sam wzorzec `flex-wrap`, co Equipment/Equipment Assignment, czy inny układ (np. zawsze pod sobą, bez zawijania)?
4. Czy istniejący punkt przełamania sidebar/hamburger na `641px` jest odpowiedni, czy zauważyłeś, że sidebar powinien się chować/zwężać już przy szerszych oknach (np. żeby zostawić więcej miejsca na tabelę)?
5. Czy to zadanie ma też obejmować formularze dodawania/edycji (dialogi Add/Edit, strony AddEquipmentHandover/AddEquipmentReturn z dwiema tabelami side-by-side), czy wyłącznie widoki list z tabelami?


## Odpowiedzi

1. Celujemy w laptopa - 1366px
2. Przedewszystkim celujemuy w zminiejszanie czcionki w całej aplikacji - wstrzymaj się z innymi zmianami.
3. Tak.
4. Zostaw jak jest w chwili obecnej.
5. Tak - zmniejszaanie czcionki.