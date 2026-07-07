# Equipment Handover Items Preview Modal

## Overview

Na liście dokumentów wydania sprzętu ("Equipment Handover") dodać możliwość podglądu pozycji (sprzętu) przypisanych do wybranego dokumentu wydania w oknie modalnym, bez konieczności przechodzenia na inną stronę. Dodatkowo uporządkować odstępy między przyciskami akcji w wierszach tabeli listy dokumentów wydania, tak by przyciski nie stykały się ze sobą.

## Problem / Motywacja

- Lista dokumentów wydania (`/equipmenthandover`) pokazuje dla każdego dokumentu tylko dane nagłówkowe (numer dokumentu, data wydania, odbiorca, kto sporządził, status zaksięgowania, status aktywności). Nie ma żadnego sposobu, żeby z tego widoku sprawdzić, jakie konkretnie pozycje sprzętu zostały wydane na danym dokumencie — trzeba by zgadywać na podstawie wydrukowanego PDF-a lub szukać informacji gdzie indziej.
- Każdy wiersz tabeli ma już kolumnę z przyciskami akcji (drukuj/podgląd PDF, załącz podpisany dokument lub pobierz załączony dokument, anuluj — w zależności od stanu dokumentu), ale przyciski te renderują się bezpośrednio obok siebie bez żadnego odstępu, przez co wizualnie się stykają i są mniej czytelne/klikalne niż pasek narzędzi nad tabelą, który już ma odpowiedni odstęp między swoimi kontrolkami.

## Cele

- Umożliwić użytkownikowi szybkie sprawdzenie z poziomu listy dokumentów wydania, jakie pozycje sprzętu są przypisane do danego dokumentu, w oknie modalnym, bez opuszczania listy.
- Ujednolicić odstępy między przyciskami akcji w wierszach tabeli listy dokumentów wydania, tak by wizualnie i funkcjonalnie dopasować je do reszty interfejsu (np. paska narzędzi nad tabelą).

## Poza zakresem (Non-Goals)

- Możliwość edycji pozycji dokumentu wydania z poziomu okna podglądu (okno ma charakter wyłącznie podglądowy/tylko do odczytu).
- Zmiana sposobu tworzenia lub dodawania pozycji do dokumentu wydania (formularz `/addequipmenthandover` pozostaje bez zmian).
- Zmiany w innych tabelach/listach w aplikacji poza listą dokumentów wydania (np. lista sprzętu, lista pracowników) — poprawka odstępów między przyciskami dotyczy wyłącznie tabeli na liście dokumentów wydania.
- Generowanie lub zmiana wydruku PDF dokumentu wydania.

## Użytkownicy i przypadki użycia

- Pracownik działu IT przeglądający listę dokumentów wydania chce szybko zweryfikować, jaki konkretnie sprzęt (producent, typ, model, numer seryjny) został wydany na wybranym dokumencie, bez konieczności pobierania/otwierania PDF-a.
- Osoba odpowiedzialna za audyt/rozliczenie majątku, przeglądając listę dokumentów wydania, chce podejrzeć zawartość kilku dokumentów pod rząd, żeby zweryfikować poprawność wydanego sprzętu — okno modalne pozwala na to bez opuszczania listy i bez utraty jej stanu (np. filtrów, przewinięcia).
- Każdy użytkownik listy korzystający z przycisków akcji w wierszu (drukuj, załącz/pobierz dokument, anuluj) klika łatwiej i trafniej, gdy przyciski mają między sobą odstęp.

## Obecny stan (punkt wyjścia)

- Lista dokumentów wydania (`EquipmentHandover.razor`, trasa `/equipmenthandover`) prezentuje tabelę z kolumnami: Lp., numer dokumentu, data wydania, odbiorca ("Do"), kto sporządził, status "zaksięgowano" (ikona), status "aktywny" (ikona) oraz kolumnę akcji.
- Kolumna akcji zawiera przyciski: drukuj/podgląd PDF (zawsze widoczny), załącz podpisany dokument (gdy dokument nie ma jeszcze załączonego pliku) albo pobierz załączony dokument (gdy plik już jest załączony) — te dwa się wykluczają, oraz przycisk anuluj (widoczny tylko dla aktywnych dokumentów). Na liście nie ma dziś przycisku edycji.
- Przyciski akcji w wierszu renderują się bezpośrednio obok siebie, bez odstępu (brak marginesu/`gap`), w przeciwieństwie do paska narzędzi nad tabelą, który już korzysta z odstępu między swoimi kontrolkami.
- Jedyne istniejące okno modalne w module dokumentów wydania to okno dołączania podpisanego dokumentu, oparte o współdzielony komponent okna modalnego używany w wielu miejscach aplikacji (Employees, Locations, Dictionaries, Equipment, Invoices). Nie istnieje dziś żadne okno modalne pokazujące wyłącznie listę pozycji (sprzętu) przypisanych do dokumentu — do odczytu.
- Dane pozycji dokumentu wydania (nazwa/model sprzętu, producent, typ sprzętu, numer seryjny, komentarz do pozycji) są już dostępne w danych dokumentu wydania pobieranych przez listę i są w tej samej, spłaszczonej postaci co pozycje pokazywane już dziś w tabeli wyboru pozycji na formularzu tworzenia dokumentu wydania (`/addequipmenthandover`) — ta ostatnia tabela może posłużyć jako wzór układu kolumn dla nowego okna podglądu.

## Wymagania funkcjonalne

1. Na liście dokumentów wydania, dla każdego wiersza (dokumentu), użytkownik ma dostęp do nowej akcji "Podgląd pozycji" (lub równoważnej), otwierającej okno modalne z listą pozycji sprzętu przypisanych do tego dokumentu.
2. Okno modalne podglądu pozycji prezentuje dla każdej pozycji przynajmniej: producenta, typ sprzętu, model oraz numer seryjny; jeśli pozycja ma komentarz, komentarz również jest widoczny.
3. Jeśli dokument wydania nie ma żadnych przypisanych pozycji, okno modalne w czytelny sposób informuje o braku pozycji (zamiast pustej tabeli).
4. Okno podglądu pozycji jest wyłącznie do odczytu — nie pozwala na dodawanie, edycję ani usuwanie pozycji dokumentu.
5. Okno podglądu pozycji można zamknąć bez wpływu na stan listy dokumentów wydania (np. bez utraty aktualnych filtrów/wyszukiwania/przewinięcia listy).
6. Nowa akcja podglądu pozycji jest dostępna niezależnie od statusu dokumentu (zaksięgowany/niezaksięgowany, aktywny/anulowany).
7. W kolumnie akcji na liście dokumentów wydania między poszczególnymi przyciskami akcji w wierszu (drukuj/podgląd PDF, załącz/pobierz dokument, anuluj, nowy przycisk podglądu pozycji) występuje wyraźny, poziomy odstęp, spójny wizualnie z odstępem już stosowanym w pasku narzędzi nad tabelą.

## Kryteria akceptacji

- Z poziomu listy dokumentów wydania da się otworzyć okno modalne pokazujące pozycje sprzętu przypisane do wybranego, konkretnego dokumentu — dane w oknie odpowiadają rzeczywistym pozycjom tego dokumentu.
- Okno podglądu pozycji nie zawiera żadnych kontrolek do edycji/dodawania/usuwania pozycji — działa wyłącznie w trybie odczytu.
- Dla dokumentu bez pozycji okno wyraźnie komunikuje, że lista jest pusta, zamiast wyświetlać pustą tabelę bez wyjaśnienia.
- Zamknięcie okna podglądu pozycji wraca użytkownika do listy dokumentów wydania z zachowanym poprzednim stanem listy (np. wyszukiwaniem).
- W tabeli listy dokumentów wydania przyciski akcji w każdym wierszu mają między sobą widoczny, poziomy odstęp — żaden przycisk nie styka się bezpośrednio z sąsiednim.
- Zmiana odstępów między przyciskami nie psuje istniejącego układu/responsywności tabeli ani nie wpływa na inne tabele/listy w aplikacji.

## Otwarte pytania

- Czy nowy przycisk "Podgląd pozycji" ma mieć własną, dedykowaną ikonę, czy wystarczy standardowa ikona "oko"/"lista" spójna z resztą przycisków akcji?
- Czy w oknie podglądu pozycji, oprócz danych pozycji (producent/typ/model/numer seryjny/komentarz), powinny się też pojawić dane nagłówkowe dokumentu (numer dokumentu, data, odbiorca), czy okno ma zawierać wyłącznie samą listę pozycji?
- Czy okno podglądu pozycji powinno umożliwiać jakiekolwiek dodatkowe akcje na liście pozycji (np. sortowanie, filtrowanie, eksport), czy ma to być prosta, statyczna tabela tylko do odczytu?
- Jaka dokładnie wartość odstępu (`gap`) między przyciskami akcji w wierszu ma być zastosowana — czy ma to być dokładnie taki sam odstęp jak w pasku narzędzi nad tabelą, czy inna, mniejsza wartość dopasowana do gęstszego układu wiersza tabeli?

---
*Uwaga: w repozytorium nie istnieje plik `_specs/template.md`, więc powyższa specyfikacja została przygotowana wg standardowej, uniwersalnej struktury dokumentu funkcjonalnego (Overview / Motywacja / Cele / Non-Goals / Użytkownicy / Obecny stan / Wymagania / Kryteria akceptacji / Otwarte pytania), tak jak przy poprzedniej specyfikacji w tym repozytorium.*
