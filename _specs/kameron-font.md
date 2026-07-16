# Zmiana Czcionki Aplikacji na Kameron

## Cel

Zmienić czcionkę używaną w całej aplikacji z obecnej IBM Plex Serif na Kameron (Google Fonts, wagi 400–700), zgodnie z linkiem wskazanym przez użytkownika: https://fonts.google.com/share?selection.family=Kameron:wght@400..700

## Kontekst / stan obecny

- Aplikacja używa dziś czcionki **IBM Plex Serif** (wprowadzonej w commicie `6da8c76`, zastępując wcześniejszą parę Orbitron/Rajdhani) dla całego tekstu — zarówno nagłówków/brandingu, jak i treści.
- Nazwa czcionki jest zdefiniowana w dokładnie jednym miejscu: `AssetSquirrelAuthorize.WebApp/wwwroot/theme-tokens.css`, w dwóch tokenach CSS: `--as-font-display` (nagłówki, branding, nagłówki tabel — używany razem z `--as-letter-spacing-wide`) i `--as-font-body` (treść). Oba tokeny wskazują dziś na `'IBM Plex Serif'`. Reszta aplikacji odwołuje się do czcionki wyłącznie przez te dwie zmienne (`var(--as-font-display)` / `var(--as-font-body)`), nigdzie nie ma zaszytej nazwy czcionki na sztywno.
- Czcionka jest ładowana z Google Fonts CDN (link `<link>` w `AssetSquirrelAuthorize.WebApp/Components/App.razor`), nie jest self-hostowana — tak samo było przy poprzedniej zmianie (Orbitron/Rajdhani → IBM Plex Serif) i użytkownik świadomie wybrał to podejście wtedy.
- **Ważne ryzyko z poprzedniej zmiany czcionki:** zamiana na IBM Plex Serif (szerszą niż poprzednia czcionka) spowodowała nakładanie się tekstu w menu bocznym (`NavMenu.razor` / `NavMenu.razor.css`) — dłuższe etykiety zaczęły zawijać się do dwóch linii i nachodzić na sąsiednie pozycje menu. Naprawiono to w dwóch krokach (commity `6da8c76` i późniejszy `464b279` — zmiana `.nav-link` na elastyczną wysokość oraz poprawka wyrównania ikon `.icon-menu`). Kameron ma inne proporcje/szerokość znaków niż IBM Plex Serif, więc podobny efekt uboczny (zawijanie etykiet, rozjazd ikon) może się powtórzyć i wymagać ponownej weryfikacji/poprawki.

## Zakres (co wchodzi)

- Podmiana nazwy czcionki w tokenach `--as-font-display` i `--as-font-body` (`theme-tokens.css`) z `'IBM Plex Serif'` na `'Kameron'` (z sensownym fallbackiem, tak jak dotychczas).
- Aktualizacja linku Google Fonts w `App.razor` na `Kameron` z wagami 400–700 (zgodnie z podanym linkiem).
- Weryfikacja wyglądu w całej aplikacji, w obu motywach (light/dark) — nagłówki, treść, tabele, dialogi, menu boczne, ekran logowania.
- Ponowna weryfikacja miejsc wrażliwych na szerokość czcionki (przede wszystkim menu boczne `NavMenu.razor`/`NavMenu.razor.css`, ale też nagłówki tabel `table-color.css`, nagłówki dialogów `dialog.css`, spinner `LoadingSpinner.razor.css`, nazwa marki na ekranie logowania `auth.css`) i poprawienie ewentualnych regresji układu (zawijanie/nakładanie tekstu, rozjazd wyrównania ikon) analogicznych do tych po poprzedniej zmianie czcionki.

## Poza zakresem (co nie wchodzi)

- Zmiana architektury systemu czcionek (nadal jeden zestaw tokenów CSS, nie wprowadzamy nowego mechanizmu) — chyba że odpowiedzi na pytania otwarte to zmienią.
- Self-hosting plików czcionki zamiast ładowania z Google Fonts CDN — zostajemy przy CDN, tak jak dotychczas, chyba że odpowiedzi na pytania otwarte to zmienią.
- Zmiana innych tokenów typograficznych niezwiązanych bezpośrednio z regresją układu (np. `--as-letter-spacing-wide`) — chyba że okaże się to konieczne do naprawienia regresji układu wywołanej nową czcionką.

## Historyjki użytkownika

- Jako użytkownik aplikacji chcę, żeby cała aplikacja używała czcionki Kameron zamiast obecnej IBM Plex Serif, żeby zmienić wygląd aplikacji na spójny z nowym wyborem typograficznym.

## Wymagania funkcjonalne

1. Cała aplikacja (nagłówki, branding, treść, tabele, dialogi, menu) używa czcionki Kameron zamiast IBM Plex Serif, w obu motywach (light/dark).
2. Czcionka Kameron jest poprawnie ładowana z Google Fonts z wagami z zakresu 400–700.
3. Brak regresji układu w miejscach wrażliwych na szerokość czcionki (menu boczne, nagłówki tabel, dialogi) — te same problemy (zawijanie/nakładanie tekstu, rozjazd wyrównania ikon), które pojawiły się przy poprzedniej zmianie czcionki, nie mogą się powtórzyć bez poprawki.
4. Czcionka jest czytelna i estetycznie spójna w obu motywach.

## Kryteria sukcesu

- Zrzuty ekranu głównych widoków aplikacji (ekran logowania z widocznym menu, przynajmniej jeden widok listy z tabelą, jeden dialog) pokazują czcionkę Kameron zamiast IBM Plex Serif, w obu motywach.
- Menu boczne i inne elementy wrażliwe na szerokość czcionki nie mają nakładającego się tekstu ani widocznie rozjechanych ikon względem etykiet.

## Pytania otwarte

1. Czy Kameron ma zastąpić IBM Plex Serif w obu tokenach (`--as-font-display` i `--as-font-body`), tak jak poprzednio zrobiono to dla całej aplikacji jedną czcionką, czy inny podział (np. Kameron tylko dla nagłówków, inna czcionka dla treści)?
2. Czy zostajemy przy ładowaniu z Google Fonts CDN (jak dotychczas), czy tym razem czcionka ma być self-hostowana pod `wwwroot/fonts` (np. z myślą o pracy offline/bez dostępu do internetu)?
3. Kameron w Google Fonts nie ma kursywy (stylu italic) — obecny link do IBM Plex Serif ładował też wagę italic 400. Czy to ma znaczenie (czy kursywa jest gdzieś w aplikacji faktycznie używana), czy można pominąć bez konsekwencji?
4. Czy przy okazji tej zmiany można też dostosować/uprościć powiązane tokeny typograficzne (np. `--as-letter-spacing-wide` używany razem z `--as-font-display` na nagłówkach), jeśli okaże się to potrzebne do dobrego wyglądu z Kameron, czy zmiana ma się ograniczyć wyłącznie do podmiany nazwy czcionki?

## Odpowiedzi

1. Tak
2. Pobierz czcionke lokalnie - na wszelki wypadek :-)
3. Podmień bez konsekwencji
4. Można
