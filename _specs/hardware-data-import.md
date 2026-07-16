
# Import Danych Sprzętu z Hardware.xlsx

## Cel

Zaimportować historyczne dane sprzętu z `AI-Data/Hardware.xlsx` (14 379 wierszy) do tabeli `Equipments` (i powiązanych słowników `HardwareTypes`/`Manufacturers`/`Suppilers`), analogicznie do już wykonanego importu `Employees`/`Locations` z Excela — tak żeby aplikacja od razu startowała z realną, historyczną bazą sprzętu, a nie danymi testowymi.

## Kontekst / stan obecny

- `Employees` (3802 wiersze) i `Locations` (156 wierszy) zostały już zaimportowane z Excela lokalnie i (do wykonania przez użytkownika) na produkcji — wszystkie inne tabele biznesowe (`Equipments`, `HardwareTypes`, `Manufacturers`, `Suppilers`, `Invoices`, `Errors` itd.) zostały wyczyszczone do zera w ramach tego samego resetu.
- `Hardware.xlsx` skonwertowany już do poprawnie zakodowanego `AI-Data/Hardware_utf8.csv` (UTF-8, ten sam znany problem z mojibake przy domyślnym eksporcie z Excela jak poprzednio).
- Kolumny w pliku: `Nr inw.`, `id_lokalizacja`, `lokalizacja`, `kod_ksg`, `KOD_SAP`, `id_typ`, `Typ sprzętu`, `id_producent`, `Producent`, `id_model`, `Model`, `id_dostawca`, `Dostawca`, `Numer faktury`, `nr_seryjny`, `id_uzytkownik`, `Użytkownik`, `OSO_KOD`, `data_zakupu`, `data_wpisania`, `opis`, `Aktywny`, `LokalizacjaAktywna`.
- Widoczne mapowanie na dzisiejszy model danych: `Nr inw.` → `Equipment.InventoryNumber`, `nr_seryjny` → `SerialNumber`, `Typ sprzętu`/`Producent`/`Dostawca` → słowniki `HardwareType`/`Manufacturer`/`Suppiler` (dopasowanie/utworzenie po nazwie), `Model` → `ModelName`, `opis` → `Description`, `Aktywny` → `IsActive`, `KOD_SAP` → `Location.Code` (ta sama kolumna co w już zaimportowanym `Locations.xlsx`), `OSO_KOD`/`kod_ksg` → `Employee.EmployeeCode` (ta sama kolumna co `OsoKod / KodKSG` w już zaimportowanym `Employees.xlsx`).
- **`Equipment.InventoryNumber` ma już wymuszony format `491`+8 cyfr i unikalność w bazie** (funkcja zaimplementowana wcześniej w tej samej sesji, `_specs/equipment-inventory-number-column.md`) — realne dane z `Nr inw.` w większości pasują do tego formatu (co potwierdza, że numeracja `491...` używana przez generator w aplikacji to kontynuacja tej samej, prawdziwej serii historycznej), ale nie wszystkie.
- `Equipment.SerialNumber` i `Equipment.ModelName` (min. 5 znaków) są `[Required]`; `SuppilerId`/`ManufacturerId`/`HardwareTypeId` są wymaganymi (nie-nullable) FK — każdy wiersz `Equipment` musi mieć wszystkie trzy przypisane.

## Zmierzone rozbieżności danych źródłowych względem dzisiejszych wymogów walidacji (istotny kontekst decyzji)

- **323 wiersze** mają puste `Nr inw.` (brak numeru inwentarzowego w danych historycznych).
- **286 wartości** `Nr inw.` występuje po 2 razy (duplikaty) — dziś `InventoryNumber` musi być unikalny.
- **1458 niepustych** wartości `Nr inw.` nie pasuje do formatu `491`+8 cyfr (inny, stary schemat numeracji, np. `X4010100122` — wygląda na inną klasę majątku, np. sieciową).
- **658 wierszy** ma puste/nieznane `nr_seryjny`, mimo że `SerialNumber` jest dziś wymagane.
- **2045 wierszy** ma `Model` krótszy niż 5 znaków (dziś `ModelName` wymaga min. 5 znaków), np. `X73S`, `T110`.
- **883 wiersze** mają `id_producent = 0` (932 z nazwą `NIEZNANY`/puste) — `ManufacturerId` jest dziś wymagane.
- **4195 wierszy** mają `id_dostawca = 0` (4335 z nazwą `NIEZNANY`/`NULL`/puste) — `SuppilerId` jest dziś wymagane. To największa dziura w danych.
- `Typ sprzętu`/`id_typ` jest praktycznie kompletny (tylko 9 wierszy z nieznaną nazwą, 0 z `id_typ=0`).
- **10156 wierszy** nie ma żadnego `KOD_SAP` (brak przypisanej lokalizacji po kodzie), a wśród 215 unikalnych kodów `KOD_SAP` obecnych w pliku, **105 nie istnieje** w już zaimportowanej tabeli `Locations` (dotyczy 1066 wierszy sprzętu) — plik `Hardware.xlsx` odwołuje się do większej liczby lokalizacji (351 unikalnych etykiet tekstowych `lokalizacja`) niż zaimportowano z `Locations.xlsx` (156 lokalizacji).
- `Aktywny`: 10161 wierszy aktywnych, 4218 nieaktywnych (część opisów wskazuje na sprzęt fizycznie zlikwidowany/złom).
- `Numer faktury` jest niepuste w 10750 wierszach — `Invoice.InvoiceNumber` jest dziś wymagane, ale poza numerem nic więcej (data/plik/użytkownik są opcjonalne), więc techniczne utworzenie rekordów `Invoice` z samego numeru jest wykonalne.
- `id_uzytkownik`/`OSO_KOD` (powiązanie z pracownikiem/użytkownikiem sprzętu) jest wypełnione tylko w ~1900 wierszach — to potencjalne dane do `EquipmentAssignment`, ale to osobny, dodatkowy krok ponad samo dodanie sprzętu.

## Zakres (co wchodzi)

- Import sprzętu do tabeli `Equipments` z danych `Hardware.xlsx`, wraz z tworzeniem/dopasowaniem potrzebnych wierszy słownikowych `HardwareTypes`, `Manufacturers`, `Suppilers` (po nazwie — jeśli nazwa już istnieje w słowniku, użyć istniejącego wiersza, inaczej utworzyć nowy).
- Dopasowanie sprzętu do już zaimportowanych `Locations` po `KOD_SAP` ↔ `Location.Code`, tam gdzie to możliwe.
- Utworzenie 5 nowych, konkretnie nazwanych lokalizacji-placeholderów (Exorigo-Upos, Exorigo - Lab Gliwice Naprawa, Exorigo INW, Lokalizacja nieznana, Zagubione) i dopasowanie do nich sprzętu na podstawie dokładnej wartości `lokalizacja` — patrz punkt 10 w Odpowiedziach. Pozostały sprzęt bez `KOD_SAP` i bez dopasowania do tych 5 kategorii zostaje z `LocationId = NULL`.
- Tworzenie/dopasowanie rekordów `Invoice` po `Numer faktury` i podłączenie `Equipment.InvoiceId`.
- Odtworzenie przypisań sprzętu do pracowników (`EquipmentAssignment`) na podstawie dopasowania `OSO_KOD`/`kod_ksg` ↔ `Employee.EmployeeCode`, wszędzie gdzie takie dopasowanie istnieje — patrz punkt 9 w Odpowiedziach dla zasad (data wydania, brak daty zwrotu, itd.).
- Rozwiązanie (zgodnie z odpowiedziami w Pytaniach otwartych) niezgodności danych źródłowych z dzisiejszymi wymogami walidacji (brakujące/duplikowane/niestandardowe numery inwentarzowe, brakujące numery seryjne, zbyt krótkie nazwy modeli, brakujący producent/dostawca).
- Rozszerzenie `MaxLength` i rozluźnienie `RegularExpression` na `Equipment.InventoryNumber`, żeby zmieściły się realne, niestandardowe historyczne numery zachowywane bez zmian (punkt 3 w Odpowiedziach) — to jedyna zmiana reguł walidacji wymagana przez ten import, nie generalne rozluźnienie dla nowego sprzętu wprowadzanego ręcznie.
- Wykonanie importu lokalnie (ja sam, bezpośrednio) oraz przygotowanie identycznego, gotowego do wykonania skryptu dla produkcji (użytkownik wykonuje sam, tak jak przy imporcie `Employees`/`Locations`).

## Poza zakresem (co nie wchodzi)

- Tworzenie nowych lokalizacji dla pozostałych ~139 innych, niedopasowanych etykiet `lokalizacja` (poza 5 wymienionymi placeholderami) — te wiersze (~7174) zostają z `LocationId = NULL`. Jeśli to nie jest wystarczające, wymaga osobnego potwierdzenia zakresu (to big deal — 139 nowych lokalizacji, nie 5).
- Zmiany w regułach walidacji `Equipment` poza samym `InventoryNumber` (np. `SerialNumber`/`ModelName` pozostają `[Required]`/`MinLength(5)` dla nowego sprzętu wprowadzanego ręcznie przez UI — import tylko transformuje dane wejściowe tak, żeby spełniały te reguły, nie zmienia ich).
- Modelowanie pełnej historii wydań/zwrotów (`EquipmentHandover`/`EquipmentReturn`) na podstawie danych z Excela — importujemy tylko bieżący stan przypisania (`EquipmentAssignment`), nie odtwarzamy dokumentów wydania/zwrotu.

## Historyjki użytkownika

- Jako administrator chcę, żeby aplikacja od pierwszego dnia miała realną listę historycznego sprzętu firmy, a nie pustą tabelę albo dane testowe.
- Jako administrator przeglądający listę sprzętu chcę widzieć rzeczywistych producentów/dostawców/typy sprzętu w filtrach i słownikach, a nie tylko te dodane ręcznie od zera.
- Jako administrator chcę wiedzieć, ile historycznych rekordów sprzętu nie udało się zaimportować "czysto" (i z jakiego powodu), żeby móc je później ręcznie uzupełnić, a nie żeby po cichu zniknęły.

## Wymagania funkcjonalne

1. Każdy zaimportowany wiersz `Equipment` musi mieć poprawny, unikalny `InventoryNumber` w formacie `491`+8 cyfr — dla wierszy bez numeru albo z numerem w innym formacie/duplikatem, zastosować rozwiązanie z Pytań otwartych (np. auto-generacja przez istniejący `InventoryNumberGenerator`, zachowanie starego numeru w `Description` dla śledzenia).
2. Każdy zaimportowany wiersz musi mieć `SerialNumber` i `ModelName` spełniające dzisiejsze wymogi walidacji (`[Required]`, min. 5 znaków) — dla wierszy z brakującymi/niewystarczającymi danymi, zastosować rozwiązanie z Pytań otwartych.
3. `HardwareType`/`Manufacturer`/`Suppiler` muszą zostać dopasowane po nazwie do istniejących słowników (jeśli nie istnieją — utworzone), tak żeby żaden wiersz `Equipment` nie został bez wymaganego FK.
4. Sprzęt z rozpoznanym `KOD_SAP` odpowiadającym istniejącej lokalizacji musi mieć ustawione `LocationId`; sprzęt bez dopasowania zostaje z `LocationId = NULL`.
5. `IsActive` na zaimportowanym sprzęcie musi odpowiadać kolumnie `Aktywny` z pliku źródłowego.
6. Import musi zostać wykonany lokalnie i udokumentowany/przygotowany do samodzielnego powtórzenia na produkcji, tak jak poprzedni import `Employees`/`Locations`.

## Kryteria sukcesu

- Liczba wierszy w `Equipments` po imporcie odpowiada liczbie wierszy z `Hardware.xlsx` zaakceptowanych do importu (z jasno policzoną i zaraportowaną liczbą odrzuconych/zmodyfikowanych wierszy i przyczyną).
- Żaden zaimportowany wiersz nie narusza dzisiejszych ograniczeń bazy (unikalność `InventoryNumber`, wymagane FK, `[Required]` pola) — import wykonuje się bez błędów SQL.
- Lista sprzętu (`/equipment`) po imporcie pokazuje realne dane (producentów, modele, numery seryjne/inwentarzowe) bez regresji w działaniu strony.
- Te same kroki dają identyczny efekt na produkcji po samodzielnym wykonaniu przygotowanego skryptu.

## Pytania otwarte

1. **323 wiersze bez numeru inwentarzowego** — auto-wygenerować nowy numer (kontynuacja serii `491...`, tym samym mechanizmem co przy ręcznym dodawaniu sprzętu w UI), czy pominąć te wiersze przy imporcie?
2. **286 zduplikowanych numerów inwentarzowych** — który z dwóch wierszy z tym samym numerem zachować oryginalny numer, a któremu wygenerować nowy (np. wg `Aktywny`/nowszej `data_wpisania`), czy inne kryterium?
3. **1458 numerów inwentarzowych w innym formacie niż `491`+8 cyfr** (inna, widocznie odrębna seria numeracji, np. `X401...`) — zachować oryginalny numer mimo niezgodności z dzisiejszą regułą formatu (wymaga decyzji: czy dla zaimportowanych danych regexowa walidacja formatu ma być pominięta), wygenerować nowy numer z zachowaniem starego gdzieś w opisie, czy pominąć te wiersze?
4. **658 wierszy bez numeru seryjnego** — zaimportować z jakimś placeholderem (np. `"BRAK"` + numer inwentarzowy dla unikalności), czy pominąć te wiersze?
5. **2045 wierszy z modelem krótszym niż 5 znaków** — dopełnić/wypełnić do wymaganej długości (jak?), czy pominąć te wiersze?
6. **4335 wierszy bez rozpoznanego dostawcy i 932 bez producenta** — utworzyć wspólny wiersz słownikowy `"NIEZNANY"` w `Suppilers`/`Manufacturers` i przypisać go tym wierszom, czy inne podejście?
7. **4218 wierszy nieaktywnych (`Aktywny = 0`, w tym opisy wskazujące na złom/likwidację)** — importować je też (jako `IsActive = false`, dla zachowania historii), czy zaimportować tylko aktywny sprzęt (10161 wierszy)?
8. **Numer faktury (10750 niepustych wierszy)** — czy w ramach tego zadania tworzyć też rekordy `Invoice` i podłączać `InvoiceId`, czy to zostaje poza zakresem (odpowiedź wpływa na sekcję "Poza zakresem" wyżej)?
9. **Przypisania do pracowników (`id_uzytkownik`/`OSO_KOD`, ~1900 wierszy)** — potwierdzenie, że to zostaje poza zakresem tego zadania (jak w sekcji "Poza zakresem"), czy chcesz to też uwzględnić?
10. **105 niedopasowanych kodów lokalizacji (`KOD_SAP`) obejmujących 1066 wierszy** — potwierdzenie, że taki sprzęt zostaje bez przypisanej lokalizacji (`LocationId = NULL`), czy wolisz inne traktowanie (np. utworzenie brakujących lokalizacji tylko z kodem, bez pełnych danych)?

1. **Wygeneruj nowe numery.** Dla 323 wierszy bez `Nr inw.` — nowy numer w formacie `491`+8 cyfr, kontynuacja serii (ten sam mechanizm co `InventoryNumberGenerator` przy ręcznym dodawaniu).
2. **Oryginalne numery dla wierszy aktywnych ze starszą datą (dodany wcześniej).** Dla 286 zduplikowanych numerów: z dwóch wierszy o tym samym `Nr inw.` wygrywa ten, który jest `Aktywny=1`; jeśli oba mają ten sam status aktywności, wygrywa ten z wcześniejszą `data_wpisania` (czyli wpisany do systemu jako pierwszy). Wygrywający wiersz zachowuje oryginalny numer; przegrywający dostaje nowo wygenerowany numer (jak w punkcie 1).
3. **Zachowaj oryginalne numery.** Dla 1458 numerów w innym formacie (np. `X4010100122`, albo `49100000079/M` z sufiksem) — numer zostaje bez zmian, **bez** wymuszania na nich formatu `491`+8 cyfr. Konsekwencja techniczna do uwzględnienia w implementacji: kolumna `Equipment.InventoryNumber` ma dziś `MaxLength(11)` — realne dane sięgają 13 znaków, więc limit długości w bazie i w encji/DTO musi zostać rozszerzony (np. do 20, z marginesem), a walidacja formatu (`RegularExpression(@"^491\d{8}$")`) musi zostać rozluźniona, żeby te konkretne, zaimportowane wiersze dało się później edytować przez UI bez błędu walidacji. To jedyne miejsce, gdzie import wymaga zmiany reguł walidacji encji `Equipment` (a nie tylko transformacji samych danych) — patrz zaktualizowana sekcja "Poza zakresem" niżej.
4. **Placeholder `"brak-numeru"`.** Dla 658 wierszy bez `nr_seryjny` — dosłowna wartość `"brak-numeru"` (ta sama dla wszystkich, nie unikalna per wiersz — `SerialNumber` nie ma dziś unikalności wymuszonej na poziomie bazy).
5. **Dodaj zera wiodące.** Dla 2045 wierszy z `Model` krótszym niż 5 znaków — dopełnienie zerami od lewej do długości 5 (np. `T110` → `0T110`, `X4S` → `00X4S`).
6. **Tak.** Wspólny wiersz `"NIEZNANY"` w `Manufacturers`/`Suppilers` dla brakującego producenta/dostawcy — w praktyce nie trzeba go tworzyć specjalnie: `Producent`/`Dostawca` już zawiera literalną wartość `"NIEZNANY"` w części wierszy (49 / 140), więc standardowe dopasowanie-po-nazwie ze scenariusza w "Zakres" #1 samo utworzy ten wiersz; wystarczy puste/`id=0` wiersze też mapować na tę samą nazwę `"NIEZNANY"`.
7. **Importuj jako nieaktywne.** 4218 wierszy z `Aktywny=0` (w tym zlikwidowane/złom) wchodzą do importu z `IsActive=false` — zgodne z istniejącą konwencją apki (sprzęt nigdy nie jest fizycznie usuwany, tylko dezaktywowany).
8. **Utwórz.** Rekordy `Invoice` tworzone/dopasowywane po `Numer faktury` (deduplikacja — jedna faktura może obejmować wiele wierszy sprzętu, więc jeden wiersz `Invoice` na unikalny numer, wiele `Equipment.InvoiceId` wskazujących na niego). Wyjątek: 8 numerów faktur przekracza `MaxLength(30)` na `Invoice.InvoiceNumber` — dla tych konkretnych wierszy `InvoiceId` zostaje `NULL` (reszta danych sprzętu importuje się normalnie), bez ucinania/fałszowania numeru faktury.
9. **Mapuj i przypisz do pracowników na podstawie osokodów wszędzie gdzie to możliwe.** To **rozszerza** oryginalny zakres (poprzednio zakładany jako "poza zakresem") — patrz zaktualizowana sekcja "Zakres" niżej. Dopasowanie po `OSO_KOD` (priorytet) albo `kod_ksg` (zapasowo) względem `Employee.EmployeeCode` — zmierzone: 5412 wierszy sprzętu ma kod pasujący do istniejącego pracownika (363 z 776 unikalnych kodów w pliku ma odpowiednika w `Employees`; pozostałe to np. zwolnieni pracownicy nieobecni już w `Employees.xlsx` — dla takich wierszy `EquipmentAssignment` po prostu nie powstaje). Dla dopasowanych wierszy: `EquipmentAssignment.DateOfHandover` = `data_wpisania`, `DateOfReturn` = `NULL` (dane źródłowe nie zawierają daty zwrotu — każde dopasowane przypisanie wychodzi jako "otwarte", niezależnie od `Aktywny` samego sprzętu; to świadome uproszczenie, nie próbujemy zgadywać daty zwrotu). `LocationId` na przypisaniu = ta sama lokalizacja co dopasowana do sprzętu (jeśli jest).
10. **Lokalizacje-placeholdery + dopasowanie.** Utworzone i dopasowane po dokładnym tekście `lokalizacja` (zmierzone w danych):
    - `Exorigo-Upos` (36 wierszy) → nowa `Location` "Exorigo-Upos", `Code = "X001"`.
    - `Exorigo - Lab Gliwice Naprawa` (2 wiersze) → nowa `Location` "Exorigo - Lab Gliwice Naprawa", `Code = "X002"`.
    - `2020 Inw Exorigo` + `2024 Inw Exorigo` (284 + 307 = 591 wierszy, dwa różne roczniki inwentaryzacji scalone w jedną lokalizację zgodnie z poleceniem) → nowa `Location` "Exorigo INW" — **brak podanego kodu w poleceniu, przyjmuję `Code = "X005"`** (kontynuacja numeracji X001–X004; do potwierdzenia/poprawienia). Rocznik (2020/2024) zachowany w `Equipment.Description` dla śledzenia.
    - `NIEZNANA` (2054 wiersze) → nowa `Location` "Lokalizacja nieznana", `Code = "X003"`.
    - `2025 sprzęt zagubiony` (296 wierszy) → nowa `Location` "Zagubione", `Code = "X004"`.
    - **Pozostałe ~7174 wiersze bez `KOD_SAP`** (139 innych, realnie wyglądających etykiet typu "2024 Inw Srebrzyńska Magazynek IT", "Likwidacje 2020/2021", "Czatolin Magazynek IT" itd. — to nazwy wewnętrznych magazynów/partii likwidacyjnych/inwentaryzacyjnych, nie sklepów z kodem SAP) **nie dostają nowej lokalizacji — zostają z `LocationId = NULL`**, zgodnie z oryginalnym domyślnym podejściem. Polecenie wymieniało tylko te 5 konkretnych kategorii — jeśli intencja była szersza (np. każda z tych 139 etykiet jako własna lokalizacja), to wymaga wyraźnego potwierdzenia, bo to bez porównania większy zakres (139 nowych lokalizacji vs 5).