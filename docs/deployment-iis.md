# Wdrożenie AssetSquirrel na IIS

Przewodnik dla działu IT: pierwsze wdrożenie oraz proces aktualizacji `AssetSquirrelAuthorize.WebApp` (Blazor Server, .NET 8) na istniejącym serwerze Windows Server z IIS.

**Cel wdrożenia:**
- Witryna dostępna pod `https://10.20.7.83:446`.
- Fizyczna lokalizacja aplikacji: `C:\inetpub\wwwroot\AssetSquirrel`.
- Baza danych: oddzielna instancja SQL Server (`komf-dc-sql.komfort.local`).

## 1. Wymagania wstępne na serwerze

- **.NET 8 Hosting Bundle** (zawiera runtime ASP.NET Core 8 oraz moduł IIS `ASP.NET Core Module V2`). Wersja musi odpowiadać SDK przypiętemu w `global.json` (8.0.422, `rollForward: latestFeature`) — wystarczy najnowszy Hosting Bundle z gałęzi .NET 8.
- W roli **Web Server (IIS)** musi być włączona funkcja **WebSocket Protocol** (Server Roles → Web Server → Application Development → WebSocket Protocol). Aplikacja to Blazor Server — każdy otwarty ekran utrzymuje długotrwałe połączenie SignalR; bez WebSockets działa (fallback na long polling), ale ze znacznie gorszą responsywnością i większym obciążeniem.
- Po instalacji Hosting Bundle wykonać `iisreset`, żeby moduł ASP.NET Core V2 się załadował.
- Uprawnienia NTFS: tożsamość puli aplikacji (patrz sekcja 2) musi mieć prawo zapisu do:
  - `C:\inetpub\wwwroot\AssetSquirrel\wwwroot\Files\Invoices` (załączniki faktur — repozytorium plików zapisuje tu na dysku),
  - katalogu `logs` używanego do stdout logging (sekcja 6), jeśli zostanie utworzony.

## 2. Konfiguracja puli aplikacji i witryny w IIS

- Nowa pula aplikacji: **.NET CLR Version = No Managed Code**, **Managed pipeline mode = Integrated**.
- Tożsamość puli: domyślnie `ApplicationPoolIdentity` wystarczy (nadać jej uprawnienia NTFS jak wyżej); jeśli polityka firmy wymaga dedykowanego konta usługowego, użyć go zamiast domyślnej tożsamości.
- Witryna wskazuje na `C:\inetpub\wwwroot\AssetSquirrel` jako ścieżkę fizyczną.
- Binding: **HTTPS, port 446**, adres IP `10.20.7.83`.
  - Certyfikat SSL musi być zaimportowany do magazynu certyfikatów maszyny (`Local Computer\Personal`) przed skonfigurowaniem bindingu.
  - Uwaga: certyfikat wystawiony na goły adres IP zwykle nie pochodzi z publicznego CA (te nie wystawiają certyfikatów na adresy IP) — należy użyć certyfikatu z wewnętrznego CA firmy albo certyfikatu self-signed zaimportowanego ręcznie na serwer i zaufanego przez klientów w sieci firmowej.
  - Rozważyć, czy dodać binding HTTP (np. na porcie 80) z przekierowaniem na HTTPS, czy od razu wymusić wyłącznie HTTPS — aplikacja i tak wywołuje `UseHttpsRedirection()`, więc binding HTTP bez przekierowania na poziomie IIS i tak trafi do aplikacji, która przekieruje dalej.

## 3. Sekrety produkcyjne — `appsettings.Production.json`

ASP.NET Core automatycznie doczytuje `appsettings.{Environment}.json` na podstawie zmiennej `ASPNETCORE_ENVIRONMENT` — domyślnie, gdy zmienna nie jest ustawiona, środowiskiem jest `Production`, więc plik `appsettings.Production.json` zostanie wczytany bez dodatkowej konfiguracji. Dla jasności zaleca się mimo to ustawić jawnie zmienną środowiskową w wygenerowanym `web.config` (sekcja `<aspNetCore><environmentVariables>`) na `ASPNETCORE_ENVIRONMENT=Production`.

**Ten plik tworzony jest ręcznie bezpośrednio na serwerze produkcyjnym (obok opublikowanych plików aplikacji) i nigdy nie trafia do repozytorium git** — `.gitignore` w repo wyklucza `appsettings.Production.json`, żeby nikt przypadkowo go nie zacommitował, gdyby tworzył go lokalnie do testów.

Wymagana zawartość (klucze — wartości uzupełnia IT bezpośrednio na serwerze, nie w repozytorium):

```json
{
  "ConnectionStrings": {
    "AssetsSquirrelIdentityAccountsDB": "<connection string do komf-dc-sql.komfort.local>"
  },
  "Smtp": {
    "Username": "<login SMTP>",
    "Password": "<hasło SMTP>"
  }
}
```

`Smtp:Host`, `Smtp:Port` i `Smtp:From` są już zdefiniowane w checked-in `appsettings.json` (`mail.markety.komfort.pl:587`) i nie trzeba ich powtarzać — tylko `Username`/`Password` są sekretami wymagającymi pliku produkcyjnego. Jeśli login/hasło SMTP używane dziś w środowisku deweloperskim (user-secrets) mają być tymi samymi danymi produkcyjnymi, wystarczy je przepisać do `appsettings.Production.json` na serwerze.

## 4. Baza danych

- Connection string musi wskazywać na `komf-dc-sql.komfort.local` i używać loginu SQL dedykowanego dla tej aplikacji.
- Login musi mieć uprawnienia wystarczające nie tylko do odczytu/zapisu danych, ale też do **zmiany schematu** (ALTER/CREATE TABLE itp.) — patrz sekcja 5, migracje EF Core są od teraz stosowane automatycznie przy starcie aplikacji.
- Sprawdzić łączność sieciową między serwerem IIS a `komf-dc-sql.komfort.local` na porcie SQL Server (domyślnie 1433, chyba że instancja używa innego portu) — firewall po obu stronach musi to dopuszczać.

## 5. Migracje EF Core — stosowane automatycznie

Aplikacja przy starcie sama nakłada oczekujące migracje EF Core na skonfigurowaną bazę danych (`Database.MigrateAsync()`), zarówno w środowisku deweloperskim, jak i produkcyjnym. Oznacza to:

- Pierwsze uruchomienie na produkcji utworzy/zaktualizuje schemat bazy automatycznie — nie trzeba ręcznie uruchamiać `dotnet ef database update`.
- Każde kolejne wdrożenie nowej wersji aplikacji z nowymi migracjami zastosuje je automatycznie przy starcie puli aplikacji — nie wymaga ręcznej interwencji, ale warto to mieć na uwadze przy planowaniu wdrożenia (aplikacja może się chwilę dłużej uruchamiać, jeśli jest dużo oczekujących migracji).
- Login SQL musi mieć odpowiednie uprawnienia (patrz sekcja 4).

## 6. Logi i diagnostyka

Dwa niezależne źródła logów:

1. **Błędy aplikacyjne** — repozytoria logują wyjątki do tabeli `Errors` w bazie danych (przez `IErrorsRepository`). Dziś nie ma w aplikacji ekranu administracyjnego do przeglądania tych wpisów — jedyny sposób to zapytanie SQL bezpośrednio do tabeli `Errors` (np. przez SQL Server Management Studio). To znane ograniczenie, nie jest częścią tego wdrożenia.
2. **Błędy startowe/hostingowe** (np. aplikacja nie startuje, błąd 500.30/502.5) — nie trafiają do tabeli `Errors`, bo aplikacja się jeszcze nie uruchomiła. Włączyć stdout logging w wygenerowanym przez publikację `web.config`:
   ```xml
   <aspNetCore processPath="dotnet" arguments=".\AssetSquirrelAuthorize.WebApp.dll"
               stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" hostingModel="InProcess">
   ```
   Utworzyć katalog `logs` w katalogu witryny i nadać do niego prawo zapisu tożsamości puli aplikacji. Dodatkowo błędy modułu ASP.NET Core V2 trafiają do **Dziennika zdarzeń Windows** (Windows Event Log), źródło "IIS AspNetCore Module V2" — pierwsze miejsce do sprawdzenia, gdy witryna zwraca błąd 500.3x zanim jeszcze cokolwiek się zaloguje do stdout.

## 7. Proces publikacji (pierwsze wdrożenie)

1. Zbudować artefakty publikacji: `dotnet publish AssetSquirrelAuthorize.WebApp -c Release -o <katalog_wyjściowy>`.
2. Skopiować zawartość `<katalog_wyjściowy>` do `C:\inetpub\wwwroot\AssetSquirrel` na serwerze.
3. Utworzyć ręcznie `appsettings.Production.json` w `C:\inetpub\wwwroot\AssetSquirrel` z realnymi wartościami (sekcja 3) — ten plik nie pochodzi z publikacji, tworzy się go osobno na serwerze.
4. Skonfigurować pulę aplikacji i witrynę w IIS (sekcja 2), w tym binding HTTPS z certyfikatem.
5. Nadać uprawnienia NTFS (sekcja 1) do `wwwroot\Files\Invoices` i do katalogu `logs`.
6. Uruchomić witrynę i zweryfikować: strona logowania się ładuje pod `https://10.20.7.83:446`, logowanie działa, dane z bazy się wyświetlają (potwierdza to poprawność connection stringa i zastosowanie migracji), wysyłka maila (np. reset hasła) działa (potwierdza to poprawność danych SMTP).

## 8. Proces aktualizacji (kolejne wdrożenia)

Ta sama aplikacja, nowa wersja kodu — checklist do powtarzania przy każdym redeployu:

1. `dotnet publish AssetSquirrelAuthorize.WebApp -c Release -o <katalog_wyjściowy>` na nowej wersji kodu.
2. W IIS Manager zatrzymać pulę aplikacji (lub umieścić plik `app_offline.htm` w katalogu witryny, żeby moduł ASP.NET Core zwalniał uchwyty do plików).
3. Skopiować nowe artefakty do `C:\inetpub\wwwroot\AssetSquirrel`, **z pominięciem katalogu `wwwroot\Files\Invoices`** — repozytorium plików faktur zapisuje pliki lokalnie w tym katalogu wewnątrz katalogu witryny, więc nadpisanie całego katalogu witryny bez wykluczenia tego podkatalogu usunie już przesłane załączniki faktur. `appsettings.Production.json` też nie jest częścią artefaktów publikacji — nie zostanie nadpisany, o ile proces kopiowania nie usuwa plików spoza publikacji (np. `robocopy` bez `/MIR`, albo z jawnym wykluczeniem `/XD` dla `Files\Invoices` i `/XF` dla `appsettings.Production.json`).
4. Usunąć `app_offline.htm` (jeśli był użyty) i uruchomić ponownie pulę aplikacji.
5. Zweryfikować działanie jak w kroku 6 sekcji 7. Migracje EF Core (jeśli nowa wersja je zawiera) zostaną zastosowane automatycznie przy tym starcie.

## Znane ograniczenia tego wdrożenia (poza zakresem)

- Brak automatyzacji CI/CD — publikacja jest procesem ręcznym wykonywanym przez IT.
- Brak ekranu administracyjnego do przeglądania błędów z tabeli `Errors` — tylko dostęp przez SQL.
- Brak strategii backupu bazy danych i katalogu `Files\Invoices` — jeśli potrzebna, powinna być osobnym tematem.
- Brak konfiguracji pod skalowanie poziome (wiele instancji IIS, sticky sessions dla obwodów Blazor Server) — to wdrożenie zakłada pojedynczą instancję.
