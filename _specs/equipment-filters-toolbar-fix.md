# Filtry na Liście Sprzętu i Poprawka Układu Paska Filtrów w Przypisaniach

## Cel

1. Naprawić układ paska filtrów na widoku Equipment Assignment (`/equipmentassignment`), gdzie przycisk wyszukiwania "ucieka" do nowego wiersza pod pozostałymi filtrami, zamiast pozostać w jednej linii z nimi.
2. Dodać na widoku Equipment (`/equipment`) filtry po dostawcy (Suppiler), producencie (Manufacturer) i typie sprzętu (Type), analogiczne do tych już istniejących na widoku Equipment Assignment.

## Kontekst / stan obecny

- **Equipment Assignment — błąd układu (patrz `AI-Data/equipment-assignment/01-error.png`):** pasek filtrów na górze widoku (`Active/Inactive`, lokalizacja, pracownik, producent, typ, pole wyszukiwania tekstowego) jest ułożony we `flex` z zawijaniem (`flex-wrap`). Gdy wszystkie filtry nie mieszczą się w jednym rzędzie, przycisk wyszukiwania (ikona lupy, część współdzielonego komponentu `TextSearchBar`) odrywa się od pola tekstowego, do którego należy, i wyświetla się samotnie w nowym rzędzie pod resztą filtrów — wygląda to na błąd, nie na zamierzony układ.
- Widok Equipment Assignment ma już dziś pełny zestaw filtrów startowych: `Active/Inactive`, Location, Employee, Manufacturer, Hardware Type oraz wyszukiwanie tekstowe (numer seryjny, numer inwentarzowy, numer faktury) — wszystkie filtrują listę przypisań na żywo.
- **Equipment — brak filtrów po Suppiler/Manufacturer/Type:** widok listy sprzętu ma dziś tylko filtr `Active/Inactive` i wyszukiwanie tekstowe (numer seryjny, model, numer inwentarzowy). Mimo że tabela wyświetla kolumny Suppiler name, Manufacturer name i Hardware type dla każdego wiersza, nie da się po nich filtrować listy — trzeba przewijać/przeszukiwać wzrokowo.
- Listy dostawców, producentów i typów sprzętu (do wypełnienia rozwijanych filtrów) są już pobierane analogicznie na widoku Equipment Assignment — ten sam wzorzec (aktywne pozycje słownikowe w `InputSelect`) może posłużyć jako punkt odniesienia dla Equipment.

## Zakres (co wchodzi)

- Poprawka układu paska filtrów na Equipment Assignment, tak żeby przycisk wyszukiwania nie odrywał się wizualnie od pola tekstowego niezależnie od szerokości okna/liczby aktywnych filtrów.
- Trzy nowe filtry na widoku Equipment: Suppiler, Manufacturer, Type (Hardware Type) — działające analogicznie do istniejącego filtru `Active/Inactive` i wyszukiwania tekstowego, tzn. zawężające na żywo listę wyświetlanego sprzętu.

## Poza zakresem (co nie wchodzi)

- Zmiana układu paska filtrów na innych widokach (Employees, Locations, Invoices, itd.) — dotyczy wyłącznie Equipment Assignment.
- Dodawanie filtra po lokalizacji lub pracowniku na widoku Equipment (Equipment Assignment już to pokrywa dla przypisanego sprzętu).
- Zmiana zestawu kolumn w tabeli Equipment lub Equipment Assignment.

## Historyjki użytkownika

- Jako użytkownik przeglądający listę przypisań sprzętu chcę, żeby pasek filtrów wyglądał spójnie i czytelnie niezależnie od szerokości okna, bez samotnie "wisącego" przycisku wyszukiwania.
- Jako użytkownik przeglądający listę sprzętu chcę móc szybko zawęzić listę do konkretnego dostawcy, producenta lub typu sprzętu, tak jak już mogę to zrobić na liście przypisań.

## Wymagania funkcjonalne

1. Na Equipment Assignment przycisk wyszukiwania (lupa) musi zawsze pozostać wizualnie połączony z polem tekstowym wyszukiwania, niezależnie od tego, ile miejsca zajmują pozostałe filtry w rzędzie.
2. Na Equipment Assignment żadny inny filtr (dropdown) nie może się rozjechać z etykietą/wartością przy zawijaniu rzędu.
3. Na Equipment należy dodać trzy nowe filtry: Suppiler, Manufacturer, Type — każdy jako rozwijana lista wypełniona aktywnymi pozycjami danego słownika (analogicznie do istniejących filtrów na Equipment Assignment), z opcją "wszystkie" (brak filtrowania po tym polu) jako wartość domyślna.
4. Nowe filtry na Equipment muszą działać łącznie z już istniejącym filtrem `Active/Inactive` i wyszukiwaniem tekstowym (zawężanie na żywo, analogicznie do `@bind-Value:after="Search"` na Equipment Assignment) — wybranie filtru odświeża listę bez przeładowania strony.
5. Zmiana wartości nowego filtru na Equipment resetuje/odświeża listę tak samo, jak dziś robi to zmiana filtru `Active/Inactive`.

## Kryteria sukcesu

- Na Equipment Assignment pasek filtrów wygląda poprawnie (bez odrywającego się przycisku wyszukiwania) przy różnych szerokościach okna i różnej liczbie aktywnych filtrów.
- Na Equipment można filtrować listę sprzętu po Suppiler, Manufacturer i Type, pojedynczo i w kombinacji z filtrem `Active/Inactive` oraz wyszukiwaniem tekstowym.
- Żadna z dotychczasowych funkcji obu widoków (dodawanie, edycja, dezaktywacja sprzętu, wyszukiwanie tekstowe, sortowanie na Equipment Assignment) nie ulega regresji.

## Pytania otwarte

1. Czy filtry Suppiler/Manufacturer/Type na Equipment powinny pokazywać wszystkie pozycje słownikowe (łącznie z nieaktywnymi), czy tylko aktywne — tak jak dziś robią to analogiczne filtry na Equipment Assignment (tylko aktywne)?
2. Czy naprawa układu paska filtrów na Equipment Assignment powinna polegać na tym, żeby pole tekstowe i przycisk wyszukiwania nigdy się nie rozdzielały (czyli razem przechodzą do nowego rzędu, jeśli nie ma miejsca), czy raczej na tym, żeby cały pasek filtrów nigdy się nie zawijał (np. przewijanie w poziomie albo układ w dwóch stałych rzędach)?
3. Czy nowe filtry Suppiler/Manufacturer/Type na Equipment powinny mieć taki sam styl/szerokość jak istniejący filtr `Active/Inactive`, czy jak analogiczne filtry na Equipment Assignment (`form-control-sm` bez dodatkowej klasy szerokości)?
4. Czy ta poprawka układu paska filtrów dotyczy tylko Equipment Assignment (jedyny widok wymieniony w zgłoszeniu), czy zauważyłeś ten sam problem też na innych widokach z wieloma filtrami (np. po dodaniu nowych filtrów Equipment ten widok może mieć podobny problem)?


## Odpowiedzi

1. Tylko aktywne
2. Pole tekstowe i przycisk w kolejnym wierszu - wyśrodkowane.
3. Analogiczne filtry na Equipment Assignment (`form-control-sm` bez dodatkowej klasy szerokości).
4. Uwazględnij w widoku Equipment.