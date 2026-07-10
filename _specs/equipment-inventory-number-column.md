# Numer Inwentarzowy w Tabeli Equipment

## Cel

Dodać do sprzętu (`Equipment`) nowe pole "Inventory number" (numer inwentarzowy, np. `49100000074`) — osobny identyfikator ewidencyjny, niezależny od numeru seryjnego producenta — i wyświetlić je jako nową kolumnę na liście sprzętu (`/equipment`).

## Kontekst / stan obecny

- Encja `AssetSquirrel.CoreBusiness.Equipment` ma już `SerialNumber` (`[Required]`, numer seryjny nadany przez producenta) i `ModelName` (`[Required]`, min. 5 znaków) — nie ma żadnego pola na wewnętrzny numer inwentarzowy/ewidencyjny.
- `EquipmentDto` (`AssetSquirrel.CoreBusiness.Dto`) 1:1 odzwierciedla pola encji plus nazwy z nawigacji (np. `SuppilerName`, `ManufacturerName`).
- Lista sprzętu (`Components/Pages/Equipment/Equipment.razor`) renderuje tabelę z kolumnami: Lp., Suppiler name, Manufacturer name, Hardware type, Model name, Serial number, Invoice number, Description, Date add, Date removed, Is active?, Added by, akcje (Edit/Dezaktywuj pod `<AdminOnly>`).
- Formularze `EquipmentAddDialogBox.razor` / `EquipmentEditDialogBox.razor` mają pola tekstowe dla `ModelName` i `SerialNumber` (proste `InputText` + `ValidationMessage`) — nowe pole powinno pójść tą samą ścieżką.
- Wyszukiwanie na liście (`TextSearchBar`, placeholder "serial number, model...") filtruje dziś po numerze seryjnym i modelu — nie wiadomo jeszcze, czy numer inwentarzowy ma się do tego dołączyć (patrz Pytania otwarte).
- Zmiana wymaga nowej migracji EF Core w `AssetsSquirrel.Plugins.EFCoreSqlServer` (kolumna w tabeli `Equipment`) — obowiązuje znana uwaga z tego repo: `dotnet ef migrations add` regularnie psuje polskie znaki w istniejących danych seed, trzeba to sprawdzić/poprawić przed zastosowaniem migracji.
- W bazie już istnieje sprzęt bez numeru inwentarzowego (dane historyczne) — trzeba zdecydować, co się z nimi dzieje po wdrożeniu tej zmiany (patrz Pytania otwarte).

## Zakres (co wchodzi)

- Nowe pole `InventoryNumber` na encji `Equipment`, DTO `EquipmentDto`, oraz migracja EF Core dodająca odpowiadającą kolumnę w tabeli `Equipment`.
- Nowa kolumna "Inventory number" na liście sprzętu (`Equipment.razor`), wyświetlająca wartość dla każdego wiersza.
- Pole do wpisania/edycji numeru inwentarzowego w `EquipmentAddDialogBox.razor` i `EquipmentEditDialogBox.razor`, z takim samym traktowaniem uprawnień jak pozostałe pola formularza (widoczne, edytowalne tylko w zakresie, w jakim cały formularz jest dziś dostępny — czyli efektywnie tylko dla roli Admin, zgodnie z istniejącym mechanizmem `<AdminOnly>`/`AdminGuard`).
- Ewentualna walidacja formatu/wymagalności numeru inwentarzowego (zakres do potwierdzenia w Pytaniach otwartych).

## Poza zakresem (co nie wchodzi)

- Zmiana istniejącego pola `SerialNumber` lub jego znaczenia — numer inwentarzowy to nowe, odrębne pole, nie zamiennik.
- Import/masowe uzupełnienie numerów inwentarzowych dla już istniejącego sprzętu w bazie (chyba że odpowiedź na pytania otwarte to zmieni).
- Zmiany w module Invoices, EquipmentHandover, EquipmentReturn, EquipmentAssignment — nowe pole pojawia się tylko w module Equipment (dodanie/edycja/lista), nie w dokumentach wydania/zwrotu/przypisania (chyba że odpowiedzi na pytania otwarte rozszerzą zakres).
- Zmiana mechanizmu wyszukiwania (`TextSearchBar`) — czy numer inwentarzowy wejdzie do wyszukiwania, to pytanie otwarte, nie założenie.

## Historyjki użytkownika

- Jako pracownik przeglądający listę sprzętu chcę widzieć numer inwentarzowy każdej pozycji, żeby móc ją jednoznacznie zidentyfikować względem fizycznej naklejki/etykiety inwentarzowej na urządzeniu.
- Jako administrator dodający nowy sprzęt chcę wpisać numer inwentarzowy razem z pozostałymi danymi sprzętu, w tym samym formularzu.
- Jako administrator edytujący istniejący sprzęt chcę móc uzupełnić lub poprawić numer inwentarzowy, jeśli nie był wcześniej wpisany albo się zmienił.

## Wymagania funkcjonalne

1. `Equipment` i `EquipmentDto` muszą mieć nowe pole na numer inwentarzowy (typ tekstowy — przykładowa wartość `49100000074` wygląda jak ciąg cyfr, ale traktowana jako tekst, nie liczba, żeby nie gubić ewentualnych zer wiodących).
2. Migracja EF Core musi dodać odpowiadającą kolumnę w tabeli `Equipment` w sposób bezpieczny dla już istniejących wierszy (patrz Pytania otwarte co do wymagalności/wartości domyślnej).
3. Lista sprzętu (`/equipment`) musi pokazywać nową kolumnę "Inventory number" dla każdego wiersza.
4. Formularze dodawania i edycji sprzętu muszą umożliwiać wpisanie/zmianę numeru inwentarzowego, z tym samym traktowaniem uprawnień (Admin) jak pozostałe pola edycyjne.
5. Wartość numeru inwentarzowego musi być poprawnie mapowana encja↔DTO (Mapster `.Adapt<T>()`, zgodnie z istniejącym wzorcem UseCase) w obu kierunkach (zapis i odczyt).

## Kryteria sukcesu

- Nowo dodany sprzęt zapisuje i wyświetla numer inwentarzowy bez błędu.
- Edycja istniejącego sprzętu pozwala uzupełnić/zmienić numer inwentarzowy, zmiana się zapisuje i jest widoczna po odświeżeniu listy.
- Kolumna "Inventory number" jest widoczna na liście sprzętu dla wszystkich ról (User widzi wartość, ale nie może jej zmienić — zgodnie z istniejącym modelem uprawnień Admin/View).
- Istniejący sprzęt (bez numeru inwentarzowego wpisanego wcześniej) nie powoduje błędu ani nie blokuje działania aplikacji po wdrożeniu migracji.

## Pytania otwarte

1. Czy numer inwentarzowy ma być wymagany (`[Required]`) dla nowego sprzętu, czy opcjonalny — a jeśli wymagany, co z już istniejącymi rekordami w bazie, które go nie mają (migracja musi ustawić im jakąś wartość domyślną, albo pole musi pozostać nullable)?
2. Czy numer inwentarzowy musi być unikalny w całej bazie (constraint), czy dopuszczamy duplikaty/puste wartości?
3. Czy jest jakiś ustalony format numeru inwentarzowego (np. zawsze 11 cyfr, jak w przykładzie `49100000074`), który trzeba zwalidować, czy pole ma przyjmować dowolny tekst?
4. Gdzie w tabeli/formularzu ma się znaleźć nowa kolumna/pole względem istniejących (np. tuż po "Serial number", czy w innym miejscu)?
5. Czy numer inwentarzowy powinien dołączyć do istniejącego wyszukiwania tekstowego na liście sprzętu (`TextSearchBar`, dziś "serial number, model...")?
6. Czy numer inwentarzowy powinien być widoczny/wykorzystany także w innych miejscach aplikacji, które dziś pokazują dane sprzętu (np. wydanie/zwrot sprzętu, przypisania, PDF-y dokumentów), czy na start tylko w module Equipment?

## Odpowiedzi

1. Numer inwentarzowy - +1 do ostatniego numeru istniejącego w bazie sprzętu / ponowne sprawdzenie pred samym zapisem Oczekiwany format w przyapdku braku: 491 XXXXXXXX - gdzie X kolejną cyfrą.
2. Numer MUSI być unikalny
3. Podany w punkic 1.
4. Druga kolumna od lewej - po Lp.
5. Tak.
6. Tylko w Equipment - to będzie kolejny krok.
