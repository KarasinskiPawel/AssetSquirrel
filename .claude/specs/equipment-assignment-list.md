# Equipment Assignment — Lista całego sprzętu

## Overview

Zbudowanie właściwej zawartości strony "Equipment Assignment" (`/equipmentassignment`), która dziś jest pustym placeholderem dostępnym z menu nawigacji. Strona ma prezentować listę całego sprzętu w systemie wraz z informacją o jego aktualnym statusie przypisania — czy jest aktualnie wydany (i komu/gdzie), czy jest dostępny (nieprzypisany).

## Problem / Motywacja

- W menu aplikacji istnieje pozycja "Equipment assignment" i strona pod adresem `/equipmentassignment`, ale jest to czysty placeholder (`<h3>Equipment Assignment</h3>` bez żadnej logiki ani danych).
- Istniejąca strona `/equipment` prezentuje inwentarz sprzętu (dane sprzętowe: producent, typ, model, numer seryjny, lokalizacja magazynowa, kto zarejestrował) w celach CRUD, ale nie pokazuje w jednym miejscu, komu dany egzemplarz sprzętu jest aktualnie przypisany/wydany.
- Informację o aktualnym przypisaniu sprzętu można dziś zobaczyć tylko pośrednio — np. przeglądając otwarte dokumenty Equipment Handover albo przez ekran wyboru sprzętu do zwrotu (`/addequipmentreturn`), który pokazuje wyłącznie sprzęt aktualnie wydany, wybranego pracownika/lokalizacji.
- Brakuje jednego, całościowego widoku odpowiadającego na pytanie "jaki mamy sprzęt w systemie i kto/co go aktualnie posiada (albo że jest wolny)".

## Cele
- Upewnij się, że wydania i zwroty sprzętu są zapisywane w tabeli EquipmentAssignment i EquipmentAssignmentHistory - jeżeli nie - napraw.
- Wyświetlić na stronie Equipment Assignment listę całego sprzętu zarejestrowanego w systemie (aktywnego), niezależnie od tego, czy jest aktualnie przypisany, czy dostępny.
- Dla każdej pozycji sprzętu pokazać jego aktualny status przypisania: przypisany (do pracownika i/lub lokalizacji, wraz z datą wydania) albo dostępny/nieprzypisany.
- Umożliwić szybkie odnalezienie konkretnej pozycji sprzętu na liście (wyszukiwanie/filtrowanie).

## Poza zakresem (Non-Goals)

- Zmiana istniejącej strony `/equipment` (inwentarz/CRUD sprzętu) — pozostaje bez zmian, jako osobna funkcja.
- Tworzenie nowego mechanizmu wydawania lub zwrotu sprzętu z poziomu tej strony — wydanie odbywa się przez istniejącą funkcję Equipment Handover, zwrot przez Equipment Return; ta specyfikacja dotyczy wyłącznie widoku/listy.
- Pełna historia wszystkich wydań/zwrotów danego egzemplarza sprzętu na przestrzeni czasu (chyba że ustalenia w toku planowania wskażą inaczej) — w pierwszej wersji chodzi o stan bieżący, nie o historię.
- Edycja danych sprzętu (producent, model, numer seryjny itd.) z poziomu tej strony.

## Użytkownicy i przypadki użycia

- Pracownik działu IT chcący szybko sprawdzić, czy dany egzemplarz sprzętu (np. po numerze seryjnym) jest aktualnie u kogoś, czy leży wolny w magazynie.
- Pracownik działu IT przygotowujący się do wydania nowego sprzętu, który chce zorientować się w całościowym stanie floty sprzętowej przed podjęciem decyzji.
- Osoba odpowiedzialna za audyt/rozliczenie majątku, która chce zobaczyć pełny obraz: ile sprzętu jest w systemie, ile jest aktualnie wydane, a ile dostępne.

## Obecny stan (punkt wyjścia)

- Strona `/equipmentassignment` istnieje i jest dostępna z menu nawigacji ("Equipment assignment"), ale nie zawiera żadnej logiki ani UI — jest to czysty placeholder.
- Encja `EquipmentAssignment` reprezentuje pojedyncze przypisanie egzemplarza sprzętu (do pracownika i/lub lokalizacji), z datą wydania (`DateOfHandover`) i opcjonalną datą zwrotu (`DateOfReturn`) — przypisanie jest "otwarte" (aktywne), dopóki nie ma ustawionej daty zwrotu.
- Repozytorium przypisań sprzętu udostępnia już metody do pobrania listy ID sprzętu aktualnie przypisanego (`GetAssignedEquipmentIdsAsync`) oraz otwartych przypisań wg zadanego warunku (`GetOpenAssignmentsAsync`).
- Encja `Equipment` (i jej DTO) zawiera pełne dane inwentarzowe egzemplarza sprzętu (producent, typ, model, numer seryjny, lokalizacja magazynowa) i jest już wykorzystywana na stronie `/equipment`.
- Sprzęt uznawany jest za dostępny do wydania, gdy jest aktywny (nieskasowany) i nie ma aktualnie otwartego przypisania — ta sama logika jest już wykorzystywana przy tworzeniu nowego dokumentu wydania.

## Wymagania funkcjonalne

1. Strona Equipment Assignment wyświetla listę całego aktywnego sprzętu zarejestrowanego w systemie, niezależnie od jego statusu przypisania.
2. Dla każdej pozycji na liście widoczne są podstawowe dane identyfikujące sprzęt (producent, typ sprzętu, model, numer seryjny).
3. Dla każdej pozycji na liście widoczny jest aktualny status przypisania: jeśli sprzęt jest przypisany — komu i/lub do jakiej lokalizacji oraz od kiedy (data wydania); jeśli nie jest przypisany — wyraźne oznaczenie jako dostępny/wolny.
4. Użytkownik może wyszukać/przefiltrować listę, aby szybko odnaleźć konkretną pozycję sprzętu.
5. Lista jest czytelna przy dużej liczbie pozycji (spójna ze sposobem prezentacji list w pozostałych częściach aplikacji, np. przewijalna tabela).

## Kryteria akceptacji

- Otwierając stronę Equipment Assignment, użytkownik widzi listę sprzętu obejmującą zarówno pozycje przypisane, jak i dostępne — bez konieczności przechodzenia na inne strony.
- Dla dowolnej pozycji przypisanej na liście widać jednoznacznie, komu i/lub do jakiej lokalizacji jest wydana oraz od kiedy.
- Dla dowolnej pozycji niedostępnej dla przypisania (bo już przypisanej) i dowolnej pozycji dostępnej — status jest wyraźnie i jednoznacznie odróżnialny na pierwszy rzut oka.
- Użytkownik może odnaleźć konkretny egzemplarz sprzętu na liście przy pomocy wyszukiwania/filtrowania, bez przewijania całej listy ręcznie.

## Otwarte pytania

- Czy lista ma pokazywać wyłącznie sprzęt aktywny (`IsActive = true`), czy również sprzęt wycofany/skasowany (`IsActive = false` / `DateRemoved` ustawione)?
- Jakie kolumny/dane powinny być widoczne dla sprzętu przypisanego poza samym "komu/gdzie i od kiedy" — np. numer dokumentu wydania, komentarz z wydania?
- Czy wyszukiwanie ma obejmować tylko dane sprzętu (producent/typ/model/numer seryjny), czy również dane osoby/lokalizacji, do której sprzęt jest przypisany?
- Czy potrzebne jest grupowanie lub sortowanie listy (np. najpierw przypisane, potem dostępne; albo alfabetycznie wg odbiorcy)?
- Czy z poziomu tej listy powinna być możliwość przejścia bezpośrednio do akcji na danej pozycji (np. link do zwrotu przypisanego sprzętu, link do dokumentu wydania), czy strona ma być czysto informacyjna (tylko do odczytu)?
- Czy lista ma być filtrowana/skopowana per lokalizacja (dla użytkowników przypisanych do jednej lokalizacji), czy zawsze pokazuje cały sprzęt w całej organizacji?

## Odpowiedzi do pytań otwartych
- automatycznie tylko sprzęt aktywny | dodatkowa opcja pozwalająca na obejrzenie sprzętu nieaktywnego - switch - aktywny - nieaktywny
- Lokalizacja | Ososba | Dostawca | Producent | Typ | Model | Numer seryjny | Numer Faktury
- Filtry dla Lokalizacja | Osoba | Producent | Typ + Okno tekstowe filtrujące po numerze seryjnym i / lub numerze faktury
- Idealnie było by dodać w tabeli przyciski pozwalające sortować po lokalizacji i osobach - alfabetycznie
- zostaw w tym momencie
- Wyświetlanie ograniczone do założonych filtrów
