# Przeniesienie Linków Logowania/Wylogowania do Górnej Belki

## Cel

Przenieść linki "Login"/"Register" (dla niezalogowanych) oraz "Logout" + dane zalogowanego użytkownika (dla zalogowanych) z bocznego menu (`NavMenu.razor`) na prawą stronę górnej belki (`top-row` w `MainLayout.razor`), tak żeby stan sesji użytkownika (kto jest zalogowany, możliwość wylogowania/zalogowania) był widoczny od razu w górnej belce, zamiast na dole listy linków nawigacyjnych w sidebarze.

## Kontekst / stan obecny

- `AssetSquirrelAuthorize.WebApp\Components\Layout\NavMenu.razor` renderuje sidebar z linkami nawigacyjnymi (Home, Equipment assignment/handover/return, Equipment, Invoices, Locations, Employees, Dictionares, opcjonalnie Users dla adminów), a na samym końcu blok `<AuthorizeView>`:
  - **Authorized**: link do `Account/Manage` z ikoną i imieniem/nazwą zalogowanego użytkownika (`@context.User.Identity?.Name`), oraz formularz POST do `Account/Logout` (przycisk "Logout" stylizowany jak link nawigacyjny, z `<AntiforgeryToken />` i polem `ReturnUrl`).
  - **NotAuthorized**: linki "Register" (`Account/Register`) i "Login" (`Account/Login`).
- `AssetSquirrelAuthorize.WebApp\Components\Layout\MainLayout.razor` renderuje górną belkę (`<div class="top-row px-4 navbar">`) zawierającą dziś tylko: link "About Us" (zewnętrzny, do dokumentacji Microsoft) i przycisk przełącznika motywu jasny/ciemny (`theme-toggle-btn`).
- CSS górnej belki (`MainLayout.razor.css`): `.top-row` ma `justify-content: flex-end` (elementy już dociśnięte do prawej strony), `height: 3.5rem`, `display:flex`, `gap:1rem`; w widoku mobilnym (`max-width: 640.98px`) zmienia się na `justify-content: space-between`. Istnieje też nieużywana dziś reguła `.top-row.auth ::deep a:first-child { flex:1; text-align:right; width:0; }` (tylko ≥641px) — pozostałość po nieużywanym dotąd warancie "auth" górnej belki z oryginalnego szablonu Blazor, potencjalnie odpowiednia dla tej zmiany.
- Strony `/Account/*` (Login, Register, Logout itd.) renderują się w tym samym `MainLayout`/sidebarze co reszta aplikacji (patrz `_specs/login-register-cleanup.md` — osobna, wcześniejsza zmiana upraszczająca same widoki Login/Register, niezależna od tej).
- Formularz Logout wymaga `<AntiforgeryToken />` i musi zostać renderowany w statycznym SSR (tak jak dziś) — przeniesienie go do `MainLayout`/górnej belki nie może naruszyć tego wymogu.

## Zakres (co wchodzi)

- Usunięcie bloku `<AuthorizeView>` (Authorized: profil+Logout; NotAuthorized: Register+Login) z końca `NavMenu.razor`.
- Dodanie analogicznego bloku do `top-row` w `MainLayout.razor`, wyrenderowanego po prawej stronie górnej belki.
- Zachowanie identycznej funkcjonalności: link do profilu z widocznym imieniem/nazwą użytkownika, przycisk Logout (formularz POST z antiforgery tokenem), linki Register/Login dla niezalogowanych.
- Dopasowanie stylu tych elementów do wizualnego kontekstu górnej belki (dziś ciemne tło `--as-topbar-bg`, linki w `--as-link-color`) — inny wygląd niż w sidebarze, bo inne otoczenie.

## Poza zakresem (co nie wchodzi)

- Zmiana logiki wylogowania/logowania (endpointy `Account/Logout`, `Account/Login`, `SignInManager` itd.) — wyłącznie przeniesienie/przestylowanie istniejącego UI.
- Zmiana wyglądu samych stron Login/Register/etc. (`Components/Account/Pages/*`) — to already covered by osobną, wcześniejszą zmianą (`_specs/login-register-cleanup.md`).
- Zmiana pozostałych linków/przycisków w sidebarze (Home, Equipment, itd.) — zostają na swoim miejscu.
- Zmiana zawartości/pozycji istniejącego linku "About Us" i przycisku przełącznika motywu — chyba że odpowiedź na pytania otwarte zmieni ich kolejność względem nowych elementów.

## Historyjki użytkownika

- Jako zalogowany użytkownik chcę widzieć swoją nazwę użytkownika i mieć dostęp do wylogowania od razu w górnej belce, a nie szukać tego na dole listy linków w sidebarze.
- Jako niezalogowany użytkownik chcę widzieć przyciski logowania/rejestracji w widocznym miejscu w górnej belce, niezależnie od tego, którą stronę aktualnie przeglądam.
- Jako administrator/IT chcę, żeby stan sesji (kto jest zalogowany) był rozpoznawalny na pierwszy rzut oka, niezależnie od tego, czy sidebar jest akurat widoczny (np. na wąskich ekranach, gdzie sidebar może być zwinięty).

## Wymagania funkcjonalne

1. Blok informacji o sesji (profil+Logout dla zalogowanych, Register+Login dla niezalogowanych) musi zostać usunięty z `NavMenu.razor` i przeniesiony do `top-row` w `MainLayout.razor`.
2. Nowy blok musi być wyrenderowany po prawej stronie górnej belki.
3. Dla zalogowanego użytkownika: widoczna nazwa/login użytkownika (`@context.User.Identity?.Name`) z linkiem do `Account/Manage`, oraz działający przycisk "Logout" (formularz POST z `<AntiforgeryToken />` do `Account/Logout`, z polem `ReturnUrl` ustawionym na aktualny URL — dziś realizowane przez `currentUrl` śledzone w `NavMenu.razor`'s `OnLocationChanged`; ta logika musi zostać przeniesiona/odtworzona w `MainLayout.razor`, bo `MainLayout` nie ma dziś takiego mechanizmu).
4. Dla niezalogowanego użytkownika: widoczne linki "Register" i "Login" (o ile odpowiedź na pytania otwarte nie zawęzi zakresu tylko do Login).
5. Zmiana nie może naruszyć działania wylogowania opisanego w `_specs`/wcześniejszych naprawach antiforgery (patrz commit „Napraw AntiforgeryValidationException przy wylogowaniu”) — formularz Logout musi zachować te same atrybuty i zachowanie.
6. Layout musi pozostać użyteczny na wąskich ekranach (mobile media query już istnieje dla `.top-row`, ale trzeba sprawdzić, czy z dodatkowymi elementami nie zacznie się zawijać/przycinać).

## Kryteria sukcesu

- Sidebar (`NavMenu.razor`) nie zawiera już linków Login/Register/Logout/danych użytkownika — kończy się na ostatnim linku nawigacyjnym (Dictionares / Users dla admina).
- Górna belka po prawej stronie pokazuje: dla zalogowanego — nazwę użytkownika + działający Logout; dla niezalogowanego — Login (i Register, jeśli w zakresie).
- Wylogowanie działa identycznie jak dziś (przekierowanie na `ReturnUrl`, brak błędu antiforgery).
- Wygląd górnej belki pozostaje czytelny i użyteczny na typowych szerokościach ekranu, w obu motywach (light/dark).

## Pytania otwarte

1. Czy "Register" ma też przenieść się do górnej belki, czy tylko "Login" (użytkownik w poleceniu wymienił explicite "Login | Logout | dane zalogowanego użytkownika", nie wspominając "Register")?
2. Jaka ma być kolejność elementów w prawej części górnej belki względem już istniejących ("About Us", przełącznik motywu) — nowe elementy na samym końcu (najbardziej po prawej), czy przed nimi?
3. Czy nazwa użytkownika + link do profilu mają wyglądać jak dotychczas (ikona + tekst, styl linku nawigacyjnego), czy dopasować się bardziej do stylu górnej belki (np. mniejszy tekst, inny kolor/hover)?
4. Czy istniejąca, nieużywana reguła CSS `.top-row.auth` (z `MainLayout.razor.css`) powinna zostać wykorzystana/dopracowana w ramach tej zmiany, czy to tylko przypadkowa pozostałość szablonu bez znaczenia?
5. Co ma się dziać na wąskich ekranach (mobile), gdzie `top-row` już zmienia `justify-content` na `space-between` — czy dane sesji mają być widoczne zawsze, czy można je np. schować/zminimalizować (sam tekst nazwy użytkownika może nie zmieścić się razem z "About Us" i przełącznikiem motywu)?

## Odpowiedzi

1. Tak
2. "About us" - usuń. Nazwisko  i imię lub email jeżeli brak | niezalogowany. Login lub Logout w zależności od stanu.
3. Dopasuj
4. Dopracuj i wykorzystaj jeżeli napradę jest niewykorzystywana.
5. Zminimalizuj - zamień na jekieś ikonki "Login" / "Logout".