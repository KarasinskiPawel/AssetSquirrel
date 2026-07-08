# Plan Wdrożenia AssetSquirrel na Serwerze IIS

## Cel

Opracować kompletny, powtarzalny plan wdrożenia aplikacji `AssetSquirrelAuthorize.WebApp` (Blazor Server, .NET 8) na istniejącym serwerze Windows Server z rolą IIS, tak aby aplikacja była dostępna w sieci firmowej pod domeną wewnętrzną z HTTPS, łączyła się z dedykowaną instancją SQL Server oraz miała poprawnie skonfigurowane sekrety, wysyłkę e-maili i monitoring błędów.

## Kontekst / stan obecny

- Aplikacja to Blazor Server (`AddInteractiveServerComponents` / `AddInteractiveServerRenderMode`), a nie WebAssembly — hosting IIS musi obsługiwać długotrwałe połączenia SignalR (WebSockets) dla obwodów (circuits) Blazor Server.
- Jedyny działający projekt webowy to `AssetSquirrelAuthorize.WebApp` (wcześniejszy szkielet `AssetSquirrel.WebApp` został usunięty z repo).
- `global.json` wymusza SDK .NET 8.0.422 (`rollForward: latestFeature`) — serwer musi mieć zainstalowany zgodny .NET 8 Hosting Bundle (runtime + moduł ASP.NET Core dla IIS).
- Baza danych to jedna wspólna `AssetsSquirrelContext : IdentityDbContext<ApplicationUser>` (Identity + dane biznesowe razem), connection string o nazwie `AssetsSquirrelIdentityAccountsDB`, dziś skonfigurowany w `appsettings.Development.json` / user-secrets (środowisko deweloperskie) — na produkcji nie może być używany `dotnet user-secrets`.
- Wysyłka e-maili (potwierdzenie konta, reset hasła) odbywa się przez `SmtpEmailSender` (`Components/Account/SmtpEmailSender.cs`, `System.Net.Mail.SmtpClient`) — host/port/from w `appsettings.json` (sekcja `Smtp`), a login/hasło do SMTP dziś trzymane w user-secrets deweloperskich.
- Aplikacja loguje błędy repozytoriów przez `IErrorsRepository.AddErrorAsync` — istnieje więc już wewnątrzaplikacyjny log błędów, niezależny od logów IIS/Windows.
- Migracje EF Core (`AssetsSquirrel.Plugins.EFCoreSqlServer`) nie były dotąd stosowane na żadnym środowisku produkcyjnym — trzeba ustalić sposób ich wdrażania na docelową bazę.
- Repozytorium plików faktur (`AssetsSquirrel.Plugins.InMemory`, mimo nazwy — tylko storage na dysku) zapisuje pliki lokalnie pod `wwwroot/Files/Invoices` — przy wdrożeniu na IIS trzeba zadbać o trwałość i uprawnienia zapisu tego katalogu.
- Ustalenia z rozmowy wstępnej:
  - Serwer docelowy: **istniejący** serwer Windows Server z już działającym IIS (kolejna witryna/app pool, nie instalacja IIS od zera).
  - Baza danych: **oddzielny** serwer/instancja SQL Server (firmowa), nie lokalnie na serwerze IIS.
  - Dostęp: aplikacja ma być dostępna pod **domeną wewnętrzną z certyfikatem SSL** (HTTPS wymagany).
  - Kluczowe do ujęcia w planie: konfiguracja sekretów (connection string, dane SMTP) przez zmienne środowiskowe / `appsettings.Production.json` zamiast user-secrets, poprawna konfiguracja SMTP na produkcji oraz logi/monitoring błędów (zarówno `IErrorsRepository`, jak i logi IIS/Windows Event Log).

## Zakres (co wchodzi)

- Lista wymagań wstępnych po stronie serwera: rola IIS, moduły wymagane dla Blazor Server (WebSockets, ASP.NET Core Module V2), .NET 8 Hosting Bundle, uprawnienia NTFS dla tożsamości puli aplikacji.
- Konfiguracja witryny i puli aplikacji w IIS dla aplikacji ASP.NET Core (tryb hostingu, tożsamość puli, ustawienia recyklingu).
- Plan publikacji aplikacji (proces budowania i przenoszenia artefaktów `dotnet publish` na serwer) — bez konkretnych poleceń/skryptów, jako opis kroków i odpowiedzialności.
- Konfiguracja domeny wewnętrznej i certyfikatu SSL (binding HTTPS w IIS) oraz ewentualne przekierowanie HTTP → HTTPS.
- Konfiguracja połączenia z oddzielnym serwerem SQL Server: connection string `AssetsSquirrelIdentityAccountsDB`, wymagania sieciowe/firewall między serwerem IIS a serwerem SQL, uprawnienia konta bazodanowego.
- Plan wdrożenia migracji EF Core na docelową bazę produkcyjną (kiedy i jak są stosowane w stosunku do wdrożenia nowej wersji aplikacji).
- Zarządzanie sekretami produkcyjnymi (connection string, dane SMTP) w sposób bezpieczny i niezależny od `dotnet user-secrets` (np. zmienne środowiskowe puli aplikacji IIS, `appsettings.Production.json` poza repozytorium, lub inny mechanizm — do ustalenia jako decyzja w ramach planu).
- Konfiguracja SMTP na produkcji, tak aby wysyłka e-maili (potwierdzenie rejestracji, reset hasła) działała poprawnie z serwera produkcyjnego.
- Trwałość katalogu załączników faktur (`wwwroot/Files/Invoices`) między wdrożeniami kolejnych wersji aplikacji.
- Plan logowania i monitoringu błędów: dostęp do logów `IErrorsRepository` (czy jest do tego widok w aplikacji), konfiguracja logów IIS i/lub Windows Event Log dla błędów startowych/hostingowych ASP.NET Core.
- Krótki opis procesu aktualizacji aplikacji do kolejnej wersji (redeploy) w oparciu o ustalony proces publikacji.

## Poza zakresem (co nie wchodzi)

- Automatyzacja CI/CD (np. GitHub Actions wdrażający bezpośrednio na serwer) — na tym etapie proces publikacji jest ręczny; automatyzacja może być tematem osobnej funkcji w przyszłości.
- Instalacja i konfiguracja samego serwera Windows Server oraz roli IIS od zera (serwer i IIS już istnieją).
- Instalacja/konfiguracja docelowego serwera SQL Server (instancja firmowa już istnieje, w zakresie jest tylko podłączenie aplikacji do niej).
- Strategia kopii zapasowych bazy danych i plików załączników (backup) — jeśli potrzebna, powinna być osobnym tematem/funkcją.
- Skalowanie poziome (load balancing, wiele instancji IIS) i związana z tym konfiguracja sticky sessions dla obwodów Blazor Server.
- Zaawansowany monitoring/APM (np. Application Insights, Grafana) — plan ogranicza się do logów już istniejących w aplikacji oraz standardowych logów IIS/Windows.

## Historyjki użytkownika

- Jako **administrator infrastruktury** chcę mieć jasną listę wymagań wstępnych na serwerze IIS, żeby przygotować środowisko przed pierwszym wdrożeniem.
- Jako **administrator infrastruktury** chcę wiedzieć, jak skonfigurować witrynę i pulę aplikacji w IIS dla aplikacji Blazor Server, żeby aplikacja działała stabilnie (bez zrywania połączeń SignalR).
- Jako **administrator infrastruktury** chcę mieć opisany sposób bezpiecznego przechowywania connection stringa do bazy i danych SMTP na produkcji, żeby nie trzymać sekretów w repozytorium ani w plikach deweloperskich.
- Jako **administrator infrastruktury** chcę mieć plan zastosowania migracji EF Core na bazie produkcyjnej, żeby wdrożenie nowej wersji nie skończyło się niespójnym schematem bazy.
- Jako **administrator infrastruktury** chcę wiedzieć, gdzie szukać logów błędów aplikacji i hostingu, żeby móc szybko zdiagnozować problem po wdrożeniu.
- Jako **użytkownik biznesowy** chcę, żeby aplikacja była dostępna pod bezpiecznym adresem HTTPS w sieci firmowej, żeby móc z niej korzystać bez ostrzeżeń przeglądarki o niezaufanym certyfikacie.

## Wymagania funkcjonalne

1. Plan musi zawierać pełną listę komponentów wymaganych na serwerze IIS (Hosting Bundle .NET 8, moduł ASP.NET Core, obsługa WebSockets) wraz z uzasadnieniem, dlaczego są potrzebne dla Blazor Server.
2. Plan musi opisywać konfigurację witryny/puli aplikacji w IIS (m.in. tryb hostingu, tożsamość puli aplikacji, uprawnienia do katalogu publikacji i do `wwwroot/Files/Invoices`).
3. Plan musi zawierać sposób konfiguracji bindingu HTTPS z certyfikatem SSL dla domeny wewnętrznej, w tym decyzję co do przekierowania z HTTP na HTTPS.
4. Plan musi opisywać sposób podania connection stringa do oddzielnego serwera SQL Server na produkcji bez trzymania go w repozytorium ani w plikach deweloperskich (np. zmienne środowiskowe puli aplikacji / `appsettings.Production.json` poza kontrolą wersji) oraz wymagania sieciowe (firewall/porty) między serwerem IIS a serwerem SQL.
5. Plan musi opisywać, kiedy i jak stosowane są migracje EF Core na bazę produkcyjną w cyklu wdrożenia (np. przed, w trakcie czy po podmianie plików aplikacji).
6. Plan musi opisywać sposób bezpiecznej konfiguracji danych SMTP na produkcji (host/port/from/login/hasło), spójny z tym, że `SmtpEmailSender` faktycznie wysyła e-maile (potwierdzenie konta, reset hasła).
7. Plan musi wskazywać, gdzie i jak sprawdzać błędy aplikacji po wdrożeniu — zarówno błędy logowane przez `IErrorsRepository`, jak i błędy startowe/hostingowe widoczne w logach IIS lub Windows Event Log.
8. Plan musi opisywać proces publikacji (`dotnet publish` → przeniesienie artefaktów na serwer → podmiana plików w katalogu witryny) w sposób na tyle jasny, żeby dało się go powtórzyć przy kolejnych aktualizacjach.
9. Plan musi wskazywać, jak zapewnić, że katalog załączników faktur przetrwa kolejne wdrożenia (nie zostanie nadpisany/wyczyszczony przy podmianie plików aplikacji).

## Kryteria sukcesu

- Osoba wdrażająca aplikację (administrator infrastruktury) może na podstawie planu samodzielnie przygotować serwer IIS od stanu "IIS już zainstalowany" do stanu "aplikacja działa pod HTTPS na domenie wewnętrznej".
- Aplikacja po wdrożeniu poprawnie łączy się z docelową, oddzielną instancją SQL Server i ma zastosowany aktualny schemat bazy (migracje EF Core).
- Wysyłka e-maili (rejestracja, reset hasła) działa na produkcji bez użycia `dotnet user-secrets`.
- Żaden sekret (connection string, dane SMTP) nie jest przechowywany w repozytorium ani w plikach commitowanych do kontroli wersji.
- Po wdrożeniu istnieje jasno opisany sposób sprawdzenia logów błędów aplikacji oraz logów hostingu w razie problemu.
- Plan opisuje również proces aktualizacji aplikacji do kolejnej wersji (redeploy) w sposób spójny z pierwszym wdrożeniem.

## Pytania otwarte

1. Jaka jest dokładna nazwa domeny wewnętrznej, pod którą aplikacja ma być dostępna, i czy certyfikat SSL już istnieje (np. wewnętrzne CA firmy), czy trzeba go dopiero wygenerować/zamówić?
2. Jaki jest dokładny adres/instancja docelowego serwera SQL Server oraz czy istnieje już konto/login dedykowany dla aplikacji, czy trzeba go założyć?
3. Czy preferowanym mechanizmem przechowywania sekretów produkcyjnych są zmienne środowiskowe skonfigurowane na poziomie puli aplikacji IIS, czy `appsettings.Production.json` trzymany poza repozytorium bezpośrednio na serwerze, czy inny mechanizm (np. Windows Credential Manager, Azure Key Vault, jeśli firma go używa)?
4. Czy migracje EF Core mają być stosowane automatycznie przy starcie aplikacji (`dbContext.Database.Migrate()`), czy ręcznie przez administratora przed każdym wdrożeniem?
5. Czy istnieje już skonfigurowane konto/serwer SMTP do użycia na produkcji, czy trzeba je dopiero uzyskać (np. od działu IT)?
6. Czy jest wymagany harmonogram/okno serwisowe na pierwsze wdrożenie (np. poza godzinami pracy), czy termin jest dowolny?
7. Kto będzie wykonywał wdrożenie w praktyce (dział IT/administrator infrastruktury) i czy ta osoba ma już dostęp administracyjny do docelowego serwera IIS i serwera SQL?

## Odpowiedzi na pytania

1. Dostępne pod 10.20.7.83:446
2. Fizyczna lokalizacja apliacji: C:\inetpub\wwwroot\AssetSquirrel
3. appsettings.Production.json
4. automatycznie
5. Ten który jest zaimplementowny w aplikacji.
6. Termin dowolony
7. IT

## Dodatkowo

1. Połączenie do serwera SQL: "Data Source=komf-dc-sql.komfort.local; Persist Security Info=True; User ID=pgsklepysync; Password=<REDACTED - patrz appsettings.Production.json na serwerze>" dostosuj tak, by link był automatycznie podstawiany na serwerach produkcyjnych.
