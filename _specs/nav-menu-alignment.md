# Wyrównanie Ikon i Etykiet w Menu Bocznym (NavMenu)

## Cel

Ujednolicić pionowe wyrównanie ikony i etykiety tekstowej dla każdej pozycji menu bocznego (`NavMenu.razor`), tak żeby wszystkie pozycje wyglądały spójnie — bez ikon, które wizualnie "skaczą" wyżej lub niżej względem tekstu w zależności od pozycji.

## Kontekst / stan obecny

- W commicie `6da8c76` ("Zmień czcionkę aplikacji na IBM Plex Serif") zmieniono czcionkę aplikacji na IBM Plex Serif, która jest szersza niż poprzednia (Rajdhani). Spowodowało to, że dłuższe etykiety menu (np. "Equipment assignment") zaczęły zawijać się do dwóch linii i nakładać na sąsiednią pozycję menu.
- Jako poprawkę tego nakładania, `.nav-link` w `NavMenu.razor.css` zmieniono z `height: 3rem; line-height: 3rem;` (stała wysokość, jedna linia) na `min-height: 3rem; line-height: 1.2; padding: 0.4rem 0;` (elastyczna wysokość, tekst może się zawinąć bez nakładania). Niezależnie użytkownik skrócił też etykietę "Equipment assignment" na "Assignment" bezpośrednio w `NavMenu.razor`.
- Zrzut ekranu `AI-Data/nav-menu/01-nav-menu.jpg` pokazuje efekt uboczny tej zmiany: ikony przy niektórych pozycjach (widoczne w kontekście m.in. "Equipment Handover", "Equipment Return") nie są już wyrównane do środka wysokości wiersza względem etykiety — ikona wydaje się przesunięta wyżej niż tekst obok niej, podczas gdy w innych pozycjach (np. "Home") ikona i tekst wyglądają na wyrównane.
- Prawdopodobna przyczyna: `.icon-menu` ma na sztywno ustawione `margin-top: 0.4rem`, dobrane pod poprzedni układ ze stałą wysokością wiersza `line-height: 3rem`. Po zmianie na elastyczną wysokość (`min-height` + `align-items: center` na kontenerze flex) ten dodatkowy, stały margines górny psuje wyśrodkowanie ikony względem tekstu, zwłaszcza gdy wysokość wiersza różni się między pozycjami.
- Menu boczne (`NavMenu.razor` / `NavMenu.razor.css`) używa wzorca: kontener `.nav-link` (`display: flex; align-items: center;`) zawierający `<span class="bi ... icon-menu">` (ikona Bootstrap Icons) i tekst etykiety obok.

## Zakres (co wchodzi)

- Ujednolicenie pionowego wyrównania ikony i etykiety we wszystkich pozycjach menu bocznego: Home, Assignment, Equipment Handover, Equipment Return, Equipment, Invoices, Locations, Employees, Dictionares oraz Users (widoczny tylko dla adminów).
- Poprawka musi działać spójnie zarówno dla etykiet jednowierszowych, jak i tych, które nadal mogą zawinąć się do dwóch linii (np. przy węższym oknie lub dłuższej etykiecie w przyszłości).
- Weryfikacja wyglądu w obu motywach (light/dark) oraz w widoku mobilnym (rozwijane menu po kliknięciu `.navbar-toggler`), nie tylko na szerokim ekranie.
- Zachowanie poprawki zawijania tekstu z commitu `6da8c76` — etykiety nadal nie mogą nakładać się na sąsiednie pozycje menu.

## Poza zakresem (co nie wchodzi)

- Zmiana zestawu ikon (Bootstrap Icons) lub kolejności pozycji menu.
- Dalsze zmiany treści etykiet poza już wprowadzonym skrótem "Assignment" (chyba że odpowiedzi na pytania otwarte to rozszerzą).
- Zmiana szerokości sidebar lub jego zachowania przy scrollowaniu (`.nav-scrollable`).
- Zmiana czcionki aplikacji (IBM Plex Serif) — to osobna, już wdrożona zmiana; ten spec dotyczy tylko efektu ubocznego w wyrównaniu ikon.

## Historyjki użytkownika

- Jako użytkownik aplikacji chcę, żeby ikony w menu bocznym były wyrównane z etykietami w spójny, przewidywalny sposób we wszystkich pozycjach, żeby menu wyglądało uporządkowanie, a nie "krzywo" czy przypadkowo.

## Wymagania funkcjonalne

1. Ikona i etykieta w każdej pozycji menu muszą być wyrównane w pionie w ten sam sposób (ten sam wzorzec wyrównania) — brak widocznej różnicy w pozycji pionowej ikony między poszczególnymi pozycjami menu.
2. Wyrównanie musi wyglądać poprawnie zarówno dla etykiet jednowierszowych, jak i dla etykiet, które zawijają się do dwóch linii.
3. Wygląd musi być spójny w obu motywach (light/dark).
4. Poprawka nie może cofnąć rozwiązania problemu nakładania się tekstu wprowadzonego w commicie `6da8c76` — etykiety nadal nie mogą nachodzić na sąsiednie pozycje menu.
5. Poprawka musi działać zarówno w widoku desktopowym (stały sidebar), jak i w widoku mobilnym (rozwijane menu).

## Kryteria sukcesu

- Zrzut ekranu całego menu bocznego pokazuje wszystkie ikony wyrównane spójnie względem swoich etykiet — żadna ikona nie jest widocznie wyżej lub niżej niż pozostałe względem swojego tekstu.
- Brak regresji: dłuższe etykiety nadal zawijają się bez nakładania na sąsiednie pozycje menu.
- Wygląd jest spójny w obu motywach i w obu układach (desktop/mobile).

## Pytania otwarte

1. Czy wyrównanie ikony powinno być "do góry" (ikona wyrównana z pierwszą linią tekstu, kolejne linie tekstu poniżej), czy "wyśrodkowane względem całego bloku tekstu" (ikona na środku wysokości, nawet jeśli tekst ma dwie linie)?
2. Czy ten sam problem dotyczy też innych miejsc w aplikacji z podobnym wzorcem ikona+tekst (np. linki "Register"/"Login" w `UserMenu.razor`), czy zakres ograniczamy wyłącznie do sidebar `NavMenu.razor`?
3. Czy przy okazji można doprecyzować/uprościć dobór rozmiaru i odstępu ikony (dziś `margin-top: 0.4rem` w `.icon-menu`, prawdopodobnie relikt starego, jednowierszowego układu), czy zmiana ma się ograniczyć wyłącznie do przywrócenia poprawnego wyrównania bez ruszania innych wartości?

## Odpowiedzi

1. Tekst wyśrodkowany względem ikony, tj. środek ikony jest jednocześnie środkiem tekstu w lini poziomej.
2. Nie, ograniczamy się do NavBar
3. Tak.