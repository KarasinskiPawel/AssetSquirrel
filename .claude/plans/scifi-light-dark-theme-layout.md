# Sci-Fi Light/Dark Theme System — Implementation Plan

## Context

Aplikacja AssetSquirrel (Blazor Server, .NET 8) ma dziś w pełni domyślny,
niezmodyfikowany wygląd Bootstrapa 5.1.0 — bez własnej tożsamości wizualnej,
bez trybu ciemnego. Spec (`_specs/scifi-light-dark-theme-layout.md`) prosi o
nowy, spójny layout wizualny w dwóch wariantach (jasny/ciemny), w bardzo
nowoczesnej, futurystycznej estetyce inspirowanej Blade Runnerem, Star
Trekiem i Gwiezdnymi Wojnami — czysto wizualna zmiana, bez ruszania logiki
biznesowej, tras czy struktury stron.

Założenia przyjęte bez odpowiedzi użytkownika (zaproponowane jako
rekomendowane, do zweryfikowania/skorygowania podczas implementacji):
- **Kierunek wizualny**: mieszanka wszystkich trzech inspiracji — trio
  akcentów: cyjan (główny), bursztyn/amber (drugorzędny), fiolet/magenta
  (rzadki akcent) — bezpieczny kompromis nawiązujący do wszystkich trzech
  bez kopiowania jednej 1:1.
- **Motyw domyślny** dla pierwszej wizyty: **ciemny** (najbardziej pasuje do
  klimatu "Blade Runner" jako wariant pokazowy), z natychmiastowym
  przełącznikiem na jasny.

Repo jest w pełni greenfield pod kątem theme'owania — zero istniejących
zmiennych CSS, zero `data-theme`, zero localStorage, zero przełącznika. Cała
logika kolorów jest dziś zaszyta na sztywno w kilku plikach.

## Zweryfikowane fakty o kodzie (potwierdzone odczytem plików)

- Bootstrap w repo to **5.1.0** (nie 5.3+) — brak wsparcia dla
  `data-bs-theme`. Nie budować theme'owania na tym mechanizmie; własna
  warstwa zmiennych CSS ponad Bootstrapem, niezależna od jego wersji.
- `Components/App.razor` — pojedynczy host document (.NET 8 "unified"
  Blazor Web App, brak `_Host.cshtml`). `<html lang="en">` bez atrybutu
  motywu. Strony `/Account/*` (Identity) renderowane są **statycznie**
  (`RenderModeForPage` zwraca `null` dla ścieżek `/Account`) — ale
  `Components/Account/Shared/AccountLayout.razor` i tak owija je w
  `MainLayout`, więc dostają ten sam sidebar/top-row co reszta aplikacji.
  `AccountLayout` wymusza pełny reload (`NavigationManager.Refresh(forceReload: true)`),
  jeśli wykryje żywy circuit — więc **przełącznik motywu na tych stronach
  nie może polegać na Blazorowej interaktywności** (żaden `@onclick`/
  `IJSRuntime` w samym przycisku przełącznika).
- `Components/Layout/MainLayout.razor` + `.razor.css` — jedyny layout
  owijający każdą stronę. Sidebar ma już gradient niebiesko-fioletowy
  (`rgb(5,39,103)→#3a0647`) — dobra baza pod ciemny motyw. `.top-row` dziś
  jasnoszary (`#f7f7f7`) z linkiem "About Us" — naturalne miejsce na
  przełącznik motywu.
- `Components/Layout/NavMenu.razor` + `.razor.css` — potwierdzone: ikony
  `-nav-menu` (`.bi-house-door-fill-nav-menu` itd., 8 sztuk) mają **na sztywno
  zaszyte białe SVG jako base64 `background-image`**, nie reagują na
  `currentColor`. Ten sam plik, kilkadziesiąt linii niżej, używa już
  poprawnego wzorca `<i class="bi bi-xxx icon-menu">` (font-glyph,
  respektuje `color`) dla innych pozycji menu — czyli naprawa polega na
  ujednoliceniu do już istniejącego, lepszego wzorca w tym samym pliku.
- `wwwroot/dialog.css` (`#dialog .dialog-container/.dialog-header/...`,
  hardcoded `whitesmoke`/`darkblue`/`black`/`blue`) i
  `wwwroot/table-color.css` (`#table-color` nagłówek `#000099`) — to są
  **ID-ki reużywane na niemal każdej stronie** (każdy modal Add/Edit, każda
  tabela listy) — najwyższa dźwignia: jedna zmiana tu przekłada się na całą
  aplikację naraz.
- `Components/Template/DialogBox.razor.css` — potwierdzone puste (0 linii).
  Zostaje puste — całe stylowanie modali zostaje scentralizowane w
  globalnym `dialog.css`, żeby nie mieć dwóch konkurujących źródeł prawdy.
- JS interop w repo to zawsze zwykłe globalne funkcje w plikach `<script>`
  ładowanych w `App.razor` (wzorzec: `wwwroot/Scripts/fileManager.js`),
  wołane przez `@inject IJSRuntime` + `InvokeVoidAsync("fn", args)` wprost w
  `.razor`. Brak `Blazored.LocalStorage`/`ProtectedLocalStorage`. Nowy
  mechanizm zapisu motywu podąża za tym samym wzorcem, ale **bez** żadnego
  wywołania przez Blazor dla samego przełącznika (patrz niżej).
- `AssetSquirrelAuthorize.WebApp.styles.css` w `App.razor` to auto-generowany
  bundle scoped-CSS (`*.razor.css`) — nigdy nie edytować ręcznie, zmiany
  scoped idą przez `MainLayout.razor.css`/`NavMenu.razor.css`.

## Architektura theme'owania

**Mechanizm**: zmienne CSS (custom properties) na `:root` (wartości jasne
domyślnie) + nadpisania pod `html[data-theme="dark"]`. Atrybut na `<html>`
(ustawiany inline-scriptem przed pierwszym malowaniem), nie na `<body>` ani
komponencie Blazor — `<html>` renderuje się raz i nie jest nadpisywany przez
diffing Blazora przy nawigacji SPA.

**Nowe pliki** (wszystkie w `wwwroot/`):
- `theme-tokens.css` — jedyne źródło prawdy dla tokenów: kolory tła/tekstu/
  akcentów (cyjan/bursztyn/fiolet), fonty, promień zaokrągleń, kolor
  poświaty (glow). Tylko zmienne, żadnych selektorów na elementy.
- `theme-components.css` — style współdzielonych powierzchni korzystających
  z tokenów: przyciski, focus-ringi na inputach, linki, `#blazor-error-ui`,
  `.as-bg-green`/`.as-bg-red`, scrollbary.
- `Scripts/theme.js` — `getStoredTheme()`, `setTheme(theme)` (ustawia
  `document.documentElement.dataset.theme` + `localStorage`),
  `toggleTheme()`, `initThemeToggleIcon()` (do synchronizacji ikony
  przycisku po stronie Blazora, czysto kosmetyczne).

**Modyfikowane pliki**:
- `Components/App.razor` — dodać blokujący inline `<script>` na samym
  początku `<head>` (przed jakimkolwiek stylesheetem) czytający
  `localStorage` i ustawiający `data-theme` na `<html>` (zapobiega
  "flashowi" złego motywu). Dodać linki do `theme-tokens.css`/
  `theme-components.css` i `Scripts/theme.js`. Dodać linki do 1-2 fontów
  (Google Fonts) dla nagłówków/UI.
- `wwwroot/dialog.css`, `wwwroot/table-color.css` — zamiana hardcoded
  kolorów na `var(--as-*)` + stylistyka sci-fi (poświata na obramowaniu
  dialogu, akcentowy nagłówek tabeli).
- `Components/Layout/MainLayout.razor.css`, `NavMenu.razor.css` — kolory
  przez tokeny; w `NavMenu.razor` zamiana 8 klas `-nav-menu` (hardcoded białe
  SVG) na wzorzec font-glyph `<i class="bi bi-xxx icon-nav">` już używany
  gdzie indziej w tym samym pliku — jednocześnie naprawia theme'owanie i
  usuwa ~8 bloków base64 SVG.
- `Components/Layout/MainLayout.razor` — dodać przycisk przełącznika motywu
  w `.top-row` (obok/zamiast linku "About Us"), jako zwykły
  `<button onclick="toggleTheme()">` (**zwykły atrybut HTML, NIE `@onclick`**
  — patrz niżej dlaczego), plus opcjonalne wywołanie
  `JSRuntime.InvokeVoidAsync("initThemeToggleIcon")` w `OnAfterRenderAsync`
  do synchronizacji ikony.

## Przełącznik motywu i zapamiętywanie wyboru

Kluczowa decyzja: **przełącznik nie może być spięty z systemem zdarzeń
Blazora** (`@onclick`/`IJSRuntime.InvokeVoidAsync` na klik) — bo strony
Identity (`/Account/*`) renderują się statycznie, bez żywego circuitu. Zwykły
atrybut HTML `onclick="toggleTheme()"` (czysty JS) działa identycznie
wszędzie, niezależnie od render mode. Dzięki temu C# w ogóle nie musi znać
aktualnego motywu — cały mechanizm to CSS + atrybut DOM + localStorage.

Przepływ: klik → `toggleTheme()` w `theme.js` przełącza
`document.documentElement.dataset.theme` i `localStorage` synchronicznie →
natychmiastowa zmiana, zero round-tripu do serwera. Blokujący inline-script
w `<head>` (patrz wyżej) zapewnia brak "mignięcia" złego motywu przy
ładowaniu i automatycznie obejmuje strony Identity (bo to czysty
HTML/JS, zero zależności od Blazora).

## Kierunek wizualny (do dopracowania podczas implementacji "na oko")

- **Typografia**: dwuczcionkowy system — font display (Orbitron/Rajdhani/
  Chakra Petch) dla nagłówków, marki, nagłówków tabel, przycisków; font body
  bardziej czytelny (Rajdhani lekki lub fallback systemowy) dla treści
  tabel/formularzy — gęstość danych biznesowych wymaga umiaru na body text.
- **Ciemny wariant** (hero/pokazowy): tło niemal czarne granatowe
  (`#0a0e14`–`#0d1117`), sidebar zachowuje/rozwija istniejący gradient
  niebiesko-fioletowy. Akcenty: cyjan (główny, linki/primary/focus/aktywny
  nav), bursztyn (drugorzędny/warning), fiolet-magenta (rzadki, akcent).
  Subtelna poświata (`box-shadow` glow) na focusowanych inputach, hover
  przycisków, aktywnym nav, obramowaniu dialogu — **umiarkowanie**, to
  aplikacja do całodniowej pracy z danymi, nie gra.
- **Jasny wariant**: czyste, chłodne jasne tło (nie czysty biały), te same
  akcenty przyciemnione/dosycone dla kontrastu na jasnym tle, poświaty
  minimalne/ograniczone do sidebaru i elementów interaktywnych.
- Cienkie obramowania, ostrzejsze/mniej zaokrąglone rogi niż domyślny
  Bootstrap (bardziej "panel HUD" niż zaokrąglona karta).

## Kolejność implementacji

1. Tokeny CSS (`theme-tokens.css`) + linki do fontów w `App.razor`.
2. Przełącznik + zapamiętywanie (inline script, `theme.js`, przycisk w
   `MainLayout.razor`) — zweryfikować end-to-end zanim inwestuje się w
   polish wizualny.
3. Layout/nav (`MainLayout.razor.css`, `NavMenu.razor.css` + naprawa ikon w
   `NavMenu.razor`) — najbardziej widoczna powierzchnia.
4. Współdzielone modale i tabele (`dialog.css`, `table-color.css`) — dzięki
   reużywanym ID-kom, jedna zmiana re-skinuje większość aplikacji naraz.
5. `theme-components.css` — przyciski/inputy/linki/status-chipy/scrollbary;
   sprawdzić czytelność iziToast i Bootstrap Icons na nowych tłach.
6. Punktowa weryfikacja 1-2 reprezentatywnych stron na wzorzec (np.
   `Equipment.razor` dla wzorca lista+tabela+dialog,
   `DictionaryHardwareType.razor` dla zagnieżdżonego słownika) — nie trzeba
   dotykać osobno pozostałych ~7 obszarów, jeśli struktura jest identyczna.
7. Ręczna weryfikacja stron logowania/rejestracji w obu motywach (dostają
   layout automatycznie z kroku 3, ale trzeba sprawdzić kontrast na
   domyślnych elementach Bootstrap Identity).

## Ryzyka

- `AssetSquirrelAuthorize.WebApp.styles.css` — nigdy nie edytować ręcznie
  (auto-generowany bundle).
- `#table-color`/`#dialog` to ID-ki reużywane na ~9+ stronach — duża
  dźwignia, ale też pojedynczy punkt awarii; testować zmiany na min. 3
  różnych stronach (Equipment, Employees, Dictionares) przed uznaniem kroku
  4 za zakończony.
- Przyciski `btn-outline-warning`/`btn-outline-danger`/`btn-outline-dark`
  (akcje edit/delete) liczą kolor z domyślnej palety Bootstrapa, nie z
  nowych tokenów — mogą wymagać jawnego override w `theme-components.css`.
- iziToast ma własne, zewnętrzne stylowanie (`izitoast.min.css`) —
  sprawdzić czytelność na nowych tłach, nadpisywać wyłącznie przez klasy w
  `theme-components.css`, nie edytować pliku vendorowego.
- Nie przesadzać z poświatą/neonem na gęstych powierzchniach danych
  (wnętrze tabel) — cel to czytelna aplikacja biznesowa całodniowego użytku,
  nie estetyka gry.

## Kluczowe pliki

- `AssetSquirrelAuthorize.WebApp/Components/App.razor`
- `AssetSquirrelAuthorize.WebApp/Components/Layout/MainLayout.razor` (+ `.razor.css`)
- `AssetSquirrelAuthorize.WebApp/Components/Layout/NavMenu.razor` (+ `.razor.css`)
- `AssetSquirrelAuthorize.WebApp/wwwroot/dialog.css`
- `AssetSquirrelAuthorize.WebApp/wwwroot/table-color.css`
- `AssetSquirrelAuthorize.WebApp/wwwroot/app.css`
- Nowe: `wwwroot/theme-tokens.css`, `wwwroot/theme-components.css`, `wwwroot/Scripts/theme.js`

## Weryfikacja

- `dotnet build AssetSquirrel.sln` — 0 błędów po każdym kroku.
- Uruchomić aplikację (`dotnet run --project AssetSquirrelAuthorize.WebApp`),
  ręcznie sprawdzić w przeglądarce: przełącznik motywu działa na stronie
  głównej, na liście (np. Equipment — tabela + dialog Add/Edit), na stronie
  logowania (`/Account/Login`) — motyw przełącza się wszędzie identycznie,
  bez migotania przy odświeżeniu strony, wybór przetrwa zamknięcie i
  ponowne otwarcie przeglądarki (localStorage).
- Sprawdzić kontrast tekstu/przycisków w obu motywach na minimum 3 różnych
  stronach (Equipment, Employees/Dictionares, Login).
- `dotnet test AssetSquirrel.UseCases.Tests` — powinno przejść bez zmian
  (czysto wizualna zmiana, zero ruszania logiki UseCases).
