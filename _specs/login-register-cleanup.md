# Uproszczenie i Wycentrowanie Widoków Login i Register

## Cel

Usunąć zbędne, nieużywane elementy z widoków logowania i rejestracji (`Login.razor`, `Register.razor`) oraz wycentrować pola formularza i przycisk, tak żeby te ekrany wyglądały spójnie z resztą aplikacji (w obu motywach) i nie prezentowały użytkownikowi informacji, które są martwe albo nieistotne w kontekście tej wewnętrznej aplikacji.

## Kontekst / stan obecny

- `Login.razor` i `Register.razor` to w dużej mierze niezmieniony szkielet wygenerowany przez szablon `dotnet new blazor --auth Individual`.
- Układ opiera się na siatce Bootstrap (`row` / `col-md-4` / `col-md-6 col-md-offset-2`) — formularz jest wyrównany do lewej strony ekranu, nie wycentrowany.
- Obie strony renderują sekcję "Use another service to log in/register" przez `ExternalLoginPicker.razor` — ale w `Program.cs` nie jest skonfigurowany żaden zewnętrzny dostawca logowania (brak `AddGoogle`/`AddMicrosoftAccount`/itp.), więc ta sekcja zawsze pokazuje tylko martwy link do ogólnego artykułu Microsoft o konfiguracji logowania zewnętrznego — czysty balast dla tej aplikacji.
- `Login.razor` dodatkowo pokazuje podtytuł "Use a local account to log in." + `<hr>` oraz linki: "Forgot your password?", "Register as a new user", "Resend email confirmation".
- `Register.razor` pokazuje podtytuł "Create a new account." + `<hr>`.
- Trasy `/Account/*` są renderowane bez interaktywności (`RenderModeForPage` w `App.razor` zwraca `null` dla ścieżek zaczynających się od `/Account`) — statyczne SSR, nie InteractiveServer. Zmiana układu musi działać w tym reżimie.
- Aplikacja korzysta z ciemnego motywu (`theme-tokens.css`, `theme-components.css`) — istnieją już ustalone wymagania dot. czytelności w trybie ciemnym dla natywnych elementów formularza.

## Zakres (co wchodzi)

- Usunięcie sekcji "Use another service to log in/register" (`ExternalLoginPicker`) z `Login.razor` i `Register.razor`.
- Usunięcie zbędnych podtytułów i separatorów ("Use a local account to log in.", "Create a new account.", towarzyszące `<hr>`), które odnosiły się do układu z dwiema kolumnami/sekcjami.
- Wycentrowanie pól formularza (Email, Password, opcjonalnie Confirm Password, Remember me) oraz przycisku submit na obu stronach — zamiast obecnego układu wyrównanego do lewej w kolumnie Bootstrap.
- Zachowanie pozostałej, potrzebnej funkcjonalności obu stron (logowanie, rejestracja, walidacja, komunikaty błędów, linki pomocnicze pozostające w zakresie po odpowiedzi na pytania otwarte).

## Poza zakresem (co nie wchodzi)

- Zmiana logiki logowania/rejestracji (`SignInManager`, `UserManager`, walidacja, wysyłka e-maila potwierdzającego) — zmiana dotyczy wyłącznie warstwy widoku.
- Zmiana innych stron `Account/*` (np. `ForgotPassword`, `ResendEmailConfirmation`, `LoginWith2fa`, `Lockout`, `RegisterConfirmation`), nawet jeśli mają ten sam nieekcentrowany układ — chyba że odpowiedź na pytania otwarte rozszerzy zakres.
- Wdrożenie faktycznych zewnętrznych dostawców logowania (Google/Microsoft itd.) — usuwamy tylko martwą sekcję UI, nie dodajemy nowej funkcjonalności.
- Zmiana globalnego motywu (`theme-tokens.css`/`theme-components.css`) poza tym, co jest potrzebne do wycentrowania tych dwóch konkretnych widoków.

## Historyjki użytkownika

- Jako użytkownik logujący się do AssetSquirrel chcę widzieć tylko pola potrzebne do zalogowania się (e-mail, hasło, "Zapamiętaj mnie", przycisk), bez sekcji o logowaniu przez inne serwisy, których i tak nie ma.
- Jako użytkownik rejestrujący nowe konto chcę widzieć krótki, wycentrowany formularz bez dodatkowych podtytułów i martwych sekcji.
- Jako administrator/IT chcę, żeby ekrany logowania i rejestracji wyglądały spójnie z resztą aplikacji (wycentrowane, czytelne w trybie ciemnym), a nie jak niedokończony szkielet wygenerowany przez szablon.

## Wymagania funkcjonalne

1. Sekcja "Use another service to log in/register" (`ExternalLoginPicker`) musi zostać usunięta z `Login.razor` i `Register.razor`.
2. Zbędne nagłówki/separatory ("Use a local account to log in.", "Create a new account.", towarzyszące `<hr>`) muszą zostać usunięte, gdy nie ma już drugiej kolumny/sekcji, do której się odnosiły.
3. Pola formularza (Email, Password, opcjonalnie Confirm Password, Remember me) oraz przycisk submit muszą być wycentrowane na ekranie — kierunek centrowania (tylko poziome vs. też pionowe) do potwierdzenia w Pytaniach otwartych.
4. Wszystkie funkcje pozostające w zakresie (logowanie, walidacja pól, komunikaty błędów przez `StatusMessage`, rejestracja, wysyłka e-maila potwierdzającego, linki pomocnicze zachowane po odpowiedzi na pytania otwarte) muszą działać identycznie jak dziś — zmiana dotyczy wyłącznie układu i usunięcia zbędnych elementów.
5. Zmiana nie może pogorszyć czytelności w trybie ciemnym (dotrzymać istniejących wymagań stylu dla tabel/formularzy w tym motywie, jeśli mają zastosowanie do tych widoków).
6. Nowy układ musi działać poprawnie w statycznym SSR (bez `InteractiveServer`), ponieważ strony `/Account/*` nie są renderowane interaktywnie.

## Kryteria sukcesu

- Widoki `/Account/Login` i `/Account/Register` nie zawierają już sekcji o logowaniu przez zewnętrzne serwisy ani zbędnych podtytułów/separatorów.
- Pola formularza i przycisk submit są wizualnie wycentrowane na obu stronach.
- Logowanie i rejestracja działają bez regresji (te same scenariusze sukcesu i błędu jak przed zmianą).
- Wygląd obu stron jest spójny z ciemnym motywem używanym w resztej aplikacji.

## Pytania otwarte

1. Czy linki "Forgot your password?", "Register as a new user" i "Resend email confirmation" na `Login.razor` mają zostać, czy są też uznawane za "niepotrzebne informacje" do usunięcia?
2. Czy wycentrowanie ma być tylko poziome (formularz jako wycentrowana kolumna/karta o ograniczonej szerokości), czy też pionowe (formularz na środku całej wysokości ekranu)?
3. Czy sekcja `ExternalLoginPicker` ma zostać usunięta z tych dwóch widoków na trwałe, czy tylko warunkowo ukryta (np. automatycznie pojawi się z powrotem, jeśli kiedyś skonfigurowany zostanie zewnętrzny dostawca)?
4. Czy inne strony pomocnicze z tym samym nieekcentrowanym układem szablonu (`ForgotPassword`, `ResendEmailConfirmation`, `LoginWith2fa`, `Lockout`, `RegisterConfirmation`) powinny zostać objęte tą samą zmianą, czy to zdecydowanie osobny temat na później?
5. Czy ma zostać dodany jakiś branding (logo/nazwa aplikacji) nad formularzem, czy wystarczy wycentrowany formularz z istniejącym nagłówkiem "Log in"/"Register"?

## Odpowiedzi

1. Linki "Forgot your password?", "Register as a new user" i "Resend email confirmation" zostają - zamień je na przyciski w stylu outline.
2. W pionie i poziomie.
3. Ukryj warunkowo.
4. Powinny zostać objęte tą samą zmianą.
5. Dodaj nazwę aplikacji z jakś małą słodką wiewiórą ;-)