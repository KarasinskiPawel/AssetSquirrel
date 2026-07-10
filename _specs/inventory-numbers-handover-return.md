# Numery Inwentarzowe w Wydaniu i Zwrocie Sprzętu

## Cel

Pokazać numer inwentarzowy (`Equipment.InventoryNumber`, wprowadzony w `_specs/equipment-inventory-number-column.md`) przy pozycjach sprzętu w module wydania (Equipment Handover) i zwrotu (Equipment Return) — na liście pozycji przy wyborze sprzętu, w podglądzie pozycji dokumentu, i na wydruku PDF — tak żeby dokument wydania/zwrotu jednoznacznie identyfikował fizyczny egzemplarz sprzętu numerem z etykiety inwentarzowej, a nie tylko numerem seryjnym producenta.

## Kontekst / stan obecny

- `Equipment.InventoryNumber` (string, wymagany, unikalny) istnieje już na encji i jest widoczny na liście `/equipment` — patrz `_specs/equipment-inventory-number-column.md` (zaimplementowane, na `master`).
- **Wydanie (Handover):** pozycje dokumentu to `EquipmentHandoverDetail` (encja z nawigacją `Equipment`), mapowane do `EquipmentHandoverDetailDto` (`ManufacturerName`, `HardwareTypeName`, `ModelName`, `SerialNumber`, `Comment` — bez `InventoryNumber`). Widoczne w trzech miejscach:
  - `Components/Pages/EquipmentHandover/AddEquipmentHandover.razor` — tabela wyboru sprzętu (z filtrami po modelu/numerze seryjnym) i tabela już wybranych pozycji.
  - `Components/Pages/EquipmentHandover/EquipmentHandoverItemsDialogBox.razor` — modal "View items" na liście `/equipmenthandover`.
  - `Services/EquipmentHandoverPdfGenerator.cs` — wydruk PDF na firmowym druku (`wwwroot/Templates/DRUK_FIRMOWY.pdf`), tabela pozycji z kolumnami Producent/Typ sprzętu/Model/Numer seryjny rysowana na ustalonych współrzędnych X (`columnX`).
- **Zwrot (Return):** pozycje dokumentu to `EquipmentAssignmentDto` (`EquipmentReturnDto.Items`), zasilane z encji `EquipmentAssignment` (nawigacja `Equipment`) — też bez `InventoryNumber` na DTO. Widoczne analogicznie w:
  - `Components/Pages/EquipmentReturn/AddEquipmentReturn.razor` — tabela wyboru przypisań do zwrotu.
  - `Components/Pages/EquipmentReturn/EquipmentReturnItemsDialogBox.razor` — modal "View items" na liście `/equipmentreturn`.
  - `Services/EquipmentReturnPdfGenerator.cs` — analogiczny wydruk PDF.
- W obu przypadkach `InventoryNumber` jest już osiągalne przez istniejącą nawigację do `Equipment` w zapytaniach repozytoriów — to nie wymaga nowej migracji ani zmian w encjach, tylko dodania pola na dwóch DTO (`EquipmentHandoverDetailDto`, `EquipmentAssignmentDto`) i rozszerzenia projekcji w repozytoriach, które je wypełniają.
- Listy dokumentów (`/equipmenthandover`, `/equipmentreturn`) pokazują dane na poziomie dokumentu (numer dokumentu, daty, odbiorca/nadawca) — numer inwentarzowy jest informacją na poziomie pozycji sprzętu w dokumencie, nie samego dokumentu.
- Layout PDF ma ograniczoną szerokość strony (firmowy druk), a kolumny pozycji już zajmują większość szerokości (`columnX` od `margin` do `margin + 380`) — dodanie kolumny wymaga zmieszczenia jej w dostępnym miejscu, ewentualnie zawężenia istniejących kolumn.

## Zakres (co wchodzi)

- Dodanie `InventoryNumber` do `EquipmentHandoverDetailDto` i `EquipmentAssignmentDto`, wraz z rozszerzeniem zapytań repozytoriów, które je zasilają (analogicznie do istniejącego `ManufacturerName`/`ModelName`/`SerialNumber`).
- Wyświetlenie numeru inwentarzowego w tabelach wyboru sprzętu przy tworzeniu wydania (`AddEquipmentHandover.razor`) i zwrotu (`AddEquipmentReturn.razor`).
- Wyświetlenie numeru inwentarzowego w podglądzie pozycji dokumentu ("View items") dla wydania i zwrotu.
- Dodanie numeru inwentarzowego do tabeli pozycji na wydruku PDF dla wydania i zwrotu.

## Poza zakresem (co nie wchodzi)

- Zmiany w samej encji `Equipment`/tabeli `Equipments` lub w module Equipment (`/equipment`) — to już zrealizowane w poprzedniej funkcji, tu tylko przenoszone dalej.
- Zmiany na listach dokumentów `/equipmenthandover` i `/equipmentreturn` (kolumny na poziomie dokumentu) — numer inwentarzowy pojawia się tylko przy pozycjach sprzętu w ramach dokumentu, nie jako nowa kolumna tych list (chyba że odpowiedź na pytania otwarte to zmieni).
- Zmiany w module Equipment Assignment (`/equipmentassignment`) jako osobnej listy — jeśli tam też pojawiają się kolumny Model/Numer seryjny, to osobny wątek, nie objęty tym zakresem (chyba że odpowiedzi na pytania otwarte to rozszerzą).
- Filtrowanie/wyszukiwanie po numerze inwentarzowym w tabelach wyboru sprzętu przy wydaniu/zwrocie — dziś jest tam wyszukiwanie po modelu/numerze seryjnym; rozszerzenie o numer inwentarzowy nie jest założeniem, tylko pytaniem otwartym.

## Historyjki użytkownika

- Jako pracownik przygotowujący dokument wydania chcę widzieć numer inwentarzowy przy każdej pozycji sprzętu, żeby wybrać właściwy fizyczny egzemplarz, a nie tylko model/numer seryjny.
- Jako pracownik przygotowujący dokument zwrotu chcę widzieć numer inwentarzowy przy zwracanych pozycjach z tego samego powodu.
- Jako osoba podpisująca wydrukowany dokument wydania/zwrotu chcę, żeby wydrukowana lista pozycji zawierała numer inwentarzowy, żeby można było zweryfikować sprzęt fizycznie po etykiecie na urządzeniu.

## Wymagania funkcjonalne

1. `EquipmentHandoverDetailDto` i `EquipmentAssignmentDto` muszą zawierać numer inwentarzowy sprzętu, wypełniany z `Equipment.InventoryNumber` przez istniejącą nawigację w zapytaniach repozytoriów.
2. Tabela wyboru sprzętu w `AddEquipmentHandover.razor` musi pokazywać numer inwentarzowy dla każdej pozycji (dostępnej do wyboru i już wybranej).
3. Tabela wyboru przypisań w `AddEquipmentReturn.razor` musi pokazywać numer inwentarzowy dla każdej pozycji.
4. Modal podglądu pozycji dokumentu wydania (`EquipmentHandoverItemsDialogBox.razor`) musi pokazywać numer inwentarzowy w tabeli pozycji.
5. Modal podglądu pozycji dokumentu zwrotu (`EquipmentReturnItemsDialogBox.razor`) musi pokazywać numer inwentarzowy w tabeli pozycji.
6. Wydruk PDF wydania (`EquipmentHandoverPdfGenerator.cs`) musi zawierać numer inwentarzowy w tabeli pozycji, w układzie, który nie powoduje ucinania/nakładania się istniejących kolumn na stronie.
7. Wydruk PDF zwrotu (`EquipmentReturnPdfGenerator.cs`) musi analogicznie zawierać numer inwentarzowy.

## Kryteria sukcesu

- Podczas tworzenia nowego dokumentu wydania i zwrotu numer inwentarzowy jest widoczny przy każdej pozycji sprzętu do wyboru.
- Otworzenie "View items" na istniejącym dokumencie wydania lub zwrotu pokazuje numer inwentarzowy każdej pozycji.
- Wygenerowany PDF wydania i zwrotu zawiera numer inwentarzowy w tabeli pozycji, czytelnie, bez nakładania się z innymi kolumnami.
- Brak regresji w istniejących przepływach tworzenia/podglądu/wydruku wydania i zwrotu.

## Pytania otwarte

1. Gdzie ma się znaleźć kolumna "Inventory number" względem istniejących kolumn (Manufacturer/Hardware type/Model/Serial number) w tabelach wyboru, podglądu i na PDF — na początku, na końcu, czy zamiast/obok numeru seryjnego?
2. Skoro miejsce na wydruku PDF jest ograniczone (firmowy druk, ustalona szerokość strony) — czy zgoda na zwężenie czcionki/kolumn istniejących pól, żeby zmieścić nową kolumnę, czy raczej usunąć jedną z istniejących kolumn (np. "Producent") z wydruku na rzecz numeru inwentarzowego?
3. Czy numer inwentarzowy powinien też dołączyć do filtrowania/wyszukiwania w tabeli wyboru sprzętu przy wydaniu (`AddEquipmentHandover.razor` ma już filtry po modelu i numerze seryjnym)?
4. Czy to samo dotyczy tabeli wyboru przypisań przy zwrocie (`AddEquipmentReturn.razor`)?
5. Czy numer inwentarzowy powinien też pojawić się w module Equipment Assignment (`/equipmentassignment`, jeśli ma własną listę z Model/Numer seryjny), czy to zostaje poza zakresem tej zmiany?


## Odpowiedzi

1. Tabele wyboru i podglądu - spróbuj dodać w kolumnie SERIAL NUMBER jeden pod drugim. Zmodyfikuj nagłówki na Serial & Inventiry Nr. Przenieś kolumnę na pierwszą pozycję W pliku pdf oddzielna kolumna, pierwsza od lewej.
2. Tak.
3. Tak
4. Tak.
5. Tak.