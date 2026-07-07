# Equipment Return

## Overview

Dokończenie funkcji "Equipment Return" (zwrot sprzętu) w AssetSquirrel: umożliwienie zarejestrowania zwrotu sprzętu, który został wcześniej wydany pracownikowi i/lub lokalizacji (poprzez funkcję "Equipment Handover"), tak aby zwrócony sprzęt ponownie stał się dostępny do wydania.

## Problem / Motywacja

- W menu aplikacji istnieje już pozycja "Equipment Return" i strona pod adresem `/equipmentreturn`, ale strona jest w praktyce pustym placeholderem — nie ma na niej żadnej funkcjonalności (brak listy, brak formularza, brak logiki).
- Dziś jedyny sposób na "zwolnienie" wydanego sprzętu to anulowanie (Cancel) całego dokumentu wydania z listy Equipment Handover — anulowanie zamyka dokument i zwalnia wszystkie pozycje sprzętu na nim naraz. Nie da się zwrócić tylko części sprzętu z danego dokumentu wydania (np. gdy pracownik oddaje jedno z dwóch wydanych urządzeń), ani zwrócić sprzętu bez jednoczesnego anulowania/unieważniania całego dokumentu wydania.
- W efekcie dział IT nie ma dziś w aplikacji sposobu na codzienną, standardową obsługę zwrotu sprzętu (np. odejście pracownika, wymiana sprzętu, zwrot po zakończeniu projektu) inaczej niż przez nadużywanie mechanizmu anulowania całego dokumentu wydania.

## Cele

- Umożliwić zarejestrowanie zwrotu jednej lub wielu konkretnych pozycji sprzętu, aktualnie wydanych pracownikowi i/lub lokalizacji, bez konieczności anulowania całego dokumentu wydania.
- Po zarejestrowaniu zwrotu, zwrócony sprzęt ma ponownie stać się dostępny do wydania na nowym dokumencie Equipment Handover.
- Zachować pełną historię wydań i zwrotów danego egzemplarza sprzętu (kto, kiedy wydał, kto, kiedy zwrócił), spójną z istniejącym mechanizmem historii przypisań 
- Wybór osoby / lokalizacji pozwala na zwrot dowolnego urządzenia które jest aktualnie doń przypisane. 
- Podczas zwrotu pracownik IT ma możliwość wskazania jednej z lokalizacji wskazanych jako magazyn jako miejsce przechowywania sprzętu.
- Umożliwić przejrzenie listy sprzętu aktualnie wydanego (nie zwróconego) wraz z informacją, komu/gdzie zostało wydane, jako punkt wyjścia do wybrania pozycji do zwrotu.

## Poza zakresem (Non-Goals)

- Zmiana istniejącego mechanizmu anulowania (Cancel) dokumentu wydania na liście Equipment Handover — pozostaje bez zmian, jako osobna, istniejąca ścieżka zamykania całego dokumentu.
- Tworzenie nowego dokumentu wydania ani edycja formularza `/addequipmenthandover` — to osobna, niezmieniana funkcja.
- Generowanie dokumentu/wydruku PDF potwierdzającego zwrot (chyba że ustalenia w toku planowania wskażą inaczej) — w pierwszej wersji funkcja może ograniczać się do zapisu zwrotu w bazie.
- Zmiana sposobu wyliczania dostępności sprzętu do wydania — istniejąca logika (sprzęt bez otwartego przypisania jest dostępny) ma zostać zachowana, zwrot ma jedynie zamykać przypisanie zgodnie z tą logiką.
- Obsługa zwrotu sprzętu, który nie został wydany przez funkcję Equipment Handover (np. sprzęt przypisany poza systemem) — poza zakresem tej specyfikacji.

## Użytkownicy i przypadki użycia

- Pracownik działu IT przyjmujący zwracany sprzęt od pracownika (np. przy odejściu z firmy lub wymianie sprzętu na inny) — wybiera zwracaną pozycję (lub kilka pozycji) sprzętu i rejestruje ich zwrot.
- Pracownik działu IT porządkujący wyposażenie lokalizacji (np. biura, które się zamyka lub zmniejsza) — rejestruje zwrot sprzętu wcześniej wydanego na tę lokalizację.
- Pracownik działu IT chcący sprawdzić, jaki sprzęt jest aktualnie "na zewnątrz" (wydany, niezwrócony) — przegląda listę aktualnie wydanego sprzętu wraz z odbiorcą, żeby zdecydować, co zwrócić.
- Osoba odpowiedzialna za audyt/rozliczenie majątku — chce zweryfikować historię wydań i zwrotów konkretnego egzemplarza sprzętu (kiedy komu wydany, kiedy i przez kogo zwrócony).

## Obecny stan (punkt wyjścia)

- Strona `/equipmentreturn` istnieje i jest dostępna z menu nawigacji, ale nie zawiera żadnej logiki ani UI — jest to czysty placeholder.
- Wydanie sprzętu (funkcja Equipment Handover) tworzy w tle rekord przypisania sprzętu (do pracownika i/lub lokalizacji) z datą wydania; przypisanie jest "otwarte" (aktywne, sprzęt uznawany za wydany) dopóki nie ma zarejestrowanej daty zwrotu.
- Jedynym istniejącym dziś mechanizmem zamykającym takie przypisanie (ustawiającym datę zwrotu) jest anulowanie całego dokumentu wydania z listy Equipment Handover — anulowanie zamyka na raz wszystkie otwarte przypisania powiązane z danym dokumentem oraz sam dokument.
- Każda zmiana stanu przypisania (wydanie, anulowanie) jest dziś odnotowywana w historii przypisań sprzętu — nowa funkcja zwrotu powinna być spójna z tym mechanizmem i również zostawiać ślad w historii.
- Dostępność sprzętu do wydania na nowym dokumencie wydania jest dziś wyliczana jako "sprzęt aktywny (nieskasowany) i bez otwartego przypisania" — sprzęt ze zwróconym (zamkniętym) przypisaniem automatycznie wraca do puli dostępnego sprzętu, bez potrzeby dodatkowego oznaczania.
- Na poziomie bazy danych każdy egzemplarz sprzętu może mieć w danym momencie co najwyżej jedno otwarte (niezwrócone) przypisanie — model danych zakłada, że sprzęt nie może być jednocześnie wydany "podwójnie".

## Wymagania funkcjonalne

1. Użytkownik może na stronie zwrotu sprzętu zobaczyć listę sprzętu aktualnie wydanego (z otwartym przypisaniem), wraz z informacją, komu i/lub do jakiej lokalizacji został wydany oraz datą wydania.
2. Użytkownik może przefiltrować/wyszukać listę aktualnie wydanego sprzętu (np. po producencie, typie, modelu, numerze seryjnym, odbiorcy), żeby szybko odnaleźć pozycję do zwrotu.
3. Użytkownik może wybrać jedną lub wiele pozycji sprzętu z listy aktualnie wydanego sprzętu i zarejestrować ich zwrot w ramach jednej operacji.
4. Po zarejestrowaniu zwrotu, dla każdej zwróconej pozycji zapisywana jest data zwrotu oraz użytkownik rejestrujący zwrot, a przypisanie tej pozycji przestaje być traktowane jako otwarte/aktywne.
5. Po zarejestrowaniu zwrotu, zwrócony sprzęt jest natychmiast ponownie widoczny jako dostępny do wybrania na nowym dokumencie wydania (`/addequipmenthandover`).
6. Zarejestrowanie zwrotu części pozycji z danego dokumentu wydania nie wpływa na pozostałe, niezwrócone jeszcze pozycje tego samego dokumentu — dokument wydania pozostaje aktywny, dopóki nie wszystkie jego pozycje zostaną zwrócone (chyba że ustalenia w toku planowania wskażą inny sposób traktowania statusu dokumentu przy częściowym zwrocie).
7. Każde zarejestrowanie zwrotu jest odnotowywane w historii przypisań sprzętu, analogicznie do istniejącego mechanizmu historii przy wydaniu i anulowaniu.
8. Nie da się zarejestrować zwrotu pozycji sprzętu, która nie ma aktualnie otwartego przypisania (np. już wcześniej zwróconej) — użytkownik dostaje czytelną informację o błędzie zamiast operacji kończącej się cichym brakiem efektu.

## Kryteria akceptacji

- Z poziomu strony zwrotu sprzętu da się zobaczyć, jaki sprzęt jest aktualnie wydany i komu/gdzie, bez konieczności przechodzenia na inne strony.
- Da się wybrać co najmniej jedną pozycję z listy aktualnie wydanego sprzętu i zarejestrować jej zwrot, po czym operacja kończy się sukcesem i widocznym potwierdzeniem.
- Po zarejestrowaniu zwrotu dana pozycja sprzętu znika z listy aktualnie wydanego sprzętu i pojawia się jako dostępna na formularzu tworzenia nowego dokumentu wydania.
- Zwrot części pozycji z wieloelementowego dokumentu wydania nie wpływa na status pozostałych, niezwróconych pozycji tego samego dokumentu.
- Historia przypisań danego egzemplarza sprzętu po zwrocie pokazuje zarówno zdarzenie wydania, jak i zdarzenie zwrotu, z poprawnymi datami.
- Próba zwrotu pozycji sprzętu, która nie jest aktualnie wydana, jest odrzucana z czytelnym komunikatem błędu.

## Otwarte pytania

- Czy strona zwrotu ma prezentować listę wydanego sprzętu płasko (wszystkie wydane pozycje razem), czy pogrupowaną według dokumentu wydania / odbiorcy?
- Co ma się dziać ze statusem samego dokumentu wydania (`IsPosted`/aktywność), gdy zwrócone zostaną wszystkie jego pozycje — czy dokument ma być automatycznie oznaczany jako zamknięty/nieaktywny, podobnie jak dziś robi to anulowanie?
- Czy zwrot ma wymagać dodatkowych danych od użytkownika poza wyborem pozycji i potwierdzeniem (np. komentarz do zwrotu, stan techniczny zwracanego sprzętu, data zwrotu inna niż "teraz")?
- Czy potrzebny jest jakiś dokument/potwierdzenie zwrotu (np. do wydruku i podpisania), analogiczny do PDF-a generowanego przy wydaniu, czy w pierwszej wersji wystarczy sam zapis w systemie?
- Czy zwrot ma być możliwy tylko dla sprzętu wydanego przez aktualnie zalogowanego użytkownika/dział IT, czy każdy uprawniony użytkownik może zarejestrować zwrot dowolnej wydanej pozycji?
- Czy potrzebne jest jakiekolwiek potwierdzenie/druga strona procesu (np. akceptacja zwrotu przez osobę, która wydawała sprzęt), czy zwrot jest jednostronną czynnością rejestrowaną od razu?

##Odpowiedzi
- Strona pozwala na wybór lokalizacji i pracownika - dopiero wtedy prezentuje wszystkie przypisane doń pozycje.
- tak
- umożliwia dodanie komentarza - umożliwia podanie daty zwrotu innej niż aktualna.
- tak - analogiczny dokument do dokumentu wydania - dostepny z tabli z listy zrówconych.
- każdu uprawnony użytkownik ma prawo przyjąć zwrot.
- zwrot jest czynnością jednostroną.

---
*Uwaga: w repozytorium nie istnieje plik `_specs/template.md`, więc powyższa specyfikacja została przygotowana wg standardowej, uniwersalnej struktury dokumentu funkcjonalnego (Overview / Motywacja / Cele / Non-Goals / Użytkownicy / Obecny stan / Wymagania / Kryteria akceptacji / Otwarte pytania), tak jak przy poprzednich specyfikacjach w tym repozytorium.*
