# Equipment Handover Document

## Overview

Dokończenie funkcji "Equipment Handover" (wydanie sprzętu) w AssetSquirrel: utworzenie pełnego dokumentu wydania sprzętu dla pracownika i/lub lokalizacji, z możliwością przypisania konkretnych pozycji (sprzętu) do tego dokumentu oraz wydrukiem/eksportem dokumentu do PDF.

## Problem / Motywacja

Moduł "Equipment Handover" istnieje dziś w aplikacji tylko częściowo:

- Strona listy (`/equipmenthandover`) ma przycisk "Handover" i pole wyszukiwania, ale wyszukiwanie nic realnie nie robi, a lista dokumentów wydania nigdy się nie wyświetla.
- Formularz tworzenia dokumentu (`/addequipmenthandover`) pozwala wybrać lokalizację docelową, pracownika docelowego i wpisać komentarz, ale sekcja "Pozycje do przekazania" (pozycje sprzętu do wydania) jest pustym placeholderem — nie da się dodać ani jednej pozycji.
- Formularz nie ma przycisku zapisu ani żadnej ścieżki zapisu do bazy — dokumentu wydania nie da się dziś w ogóle utworzyć i zapisać.
- Model danych (encje `EquipmentHandover`/`EquipmentHandoverDetail`) oraz warstwa dostępu do danych już częściowo istnieją (m.in. numer dokumentu, data wydania, flaga "zaksięgowano"/"posted", pozycje), ale nie są w pełni wykorzystane przez UI.
- Nie istnieje żadna funkcja generowania/drukowania dokumentu PDF w całej aplikacji — dla tego dokumentu miałaby to być pierwsza taka funkcja.

W efekcie dział IT nie ma dziś w aplikacji żadnego sposobu na formalne udokumentowanie wydania sprzętu pracownikowi lub do lokalizacji, mimo że reszta procesu (ewidencja sprzętu, przypisania, zwroty) jest już obsługiwana.

## Cele

- Umożliwić utworzenie kompletnego dokumentu wydania sprzętu: wybór odbiorcy (pracownik i/lub lokalizacja), data, komentarz oraz lista konkretnych pozycji sprzętu wydawanych na tym dokumencie.
- Umożliwić przypisanie (dodanie/usunięcie) pozycji sprzętu do tworzonego dokumentu wydania, z listy sprzętu dostępnego w systemie.
- Umożliwić zapisanie utworzonego dokumentu wydania w bazie danych.
- Umożliwić wygenerowanie/wydrukowanie zapisanego dokumentu wydania w formacie PDF, zawierającego wszystkie kluczowe informacje (numer dokumentu, strony wydania/odbioru, data, lista pozycji, komentarz) — gotowego do wydruku i podpisania.
- Umożliwić odnalezienie i podgląd wcześniej utworzonych dokumentów wydania (lista, wyszukiwanie po numerze dokumentu).
- Umożliwić edycję / anulowanie już "zaksięgowanego" (posted) dokumentu wydania.

## Poza zakresem (Non-Goals)

- Elektroniczny podpis dokumentu (podpis odręczny na wydruku pozostaje poza zakresem).
- Zmiana modelu przypisania sprzętu do pracownika (`/equipmentassignment`) — to osobna, odrębna funkcja; niniejsza specyfikacja dotyczy wyłącznie dokumentu wydania.
- Proces zwrotu sprzętu (`Equipment Return`) — traktowany jako osobna funkcja, nieobjęta tym dokumentem.
- Powiadomienia e-mail o wydaniu sprzętu.
- Masowe/wsadowe tworzenie wielu dokumentów wydania naraz.

## Użytkownicy i przypadki użycia

- Pracownik działu IT wydający sprzęt nowemu lub obecnemu pracownikowi — tworzy dokument wydania, dodaje pozycje sprzętu, zapisuje i drukuje dokument do podpisu.
- Pracownik działu IT wyposażający nową lokalizację/biuro w sprzęt — tworzy dokument wydania skierowany do lokalizacji zamiast konkretnej osoby.
- Osoba odpowiedzialna za audyt/rozliczenie majątku — odnajduje wcześniej wystawiony dokument wydania po numerze, żeby zweryfikować, jaki sprzęt i kiedy został komu wydany.
- możliwość jednoczesnego przypisanie pracownika i lokalizacji

## Obecny stan (punkt wyjścia)

- Istnieje strona listy dokumentów wydania oraz strona formularza tworzenia nowego dokumentu, ale obie są w praktyce niefunkcjonalne (patrz sekcja "Problem / Motywacja").
- Model danych dopuszcza wydanie zarówno "do pracownika", jak i "do lokalizacji", a także (teoretycznie) rejestruje stronę wydającą (lokalizację/pracownika źródłowego) — do ustalenia w toku planowania, na ile ta strona wydania ma być widoczna/wymagana w UI.
- Istnieje pole/flaga oznaczająca, że dokument został "zaksięgowany" (posted) — sugeruje to zamierzony cykl życia dokumentu (np. wersja robocza → zatwierdzona/zaksięgowana), którego szczegóły wymagają doprecyzowania.
- Pozycje dokumentu wydania są dziś modelowane jako odniesienie do *rodzaju* sprzętu (typu urządzenia), a nie do konkretnej, fizycznej sztuki sprzętu (np. po numerze seryjnym) — do rozstrzygnięcia, czy dokument wydania ma referować konkretne egzemplarze sprzętu z ewidencji, czy tylko typy/ilości.

## Wymagania funkcjonalne

1. Użytkownik może rozpocząć tworzenie nowego dokumentu wydania z listy dokumentów wydania.
2. Użytkownik wskazuje odbiorcę dokumentu: pracownika i/lub lokalizację.
3. Użytkownik może dodać dowolną liczbę pozycji sprzętu do tworzonego dokumentu, wybierając je z listy dostępnego (aktywnego, niewydanego) sprzętu w systemie.
4. Użytkownik może przefiltrować listę dostępnego sprzętu (po producencie, typie sprzętu, modelu, numerze seryjnym), żeby szybko odnaleźć pozycję do dodania. Lista dostępnego sprzętu (z filtrami, lewa połowa ekranu) i lista już wybranych pozycji dla tworzonego dokumentu (prawa połowa ekranu) są prezentowane obok siebie, żeby ułatwić porównanie i unikać przewijania.
5. Użytkownik może usunąć wcześniej dodaną pozycję z dokumentu przed jego zapisaniem.
6. Użytkownik może wpisać opcjonalny komentarz do całego dokumentu.
7. Po zapisaniu dokument otrzymuje unikalny numer dokumentu i datę wydania.
8. Po zapisaniu dokumentu użytkownik może wygenerować/pobrać jego wersję w formacie PDF, zawierającą wszystkie dane dokumentu i listę wydanych pozycji, w formie nadającej się do wydruku i podpisania przez obie strony.
9. Użytkownik może wyszukać wcześniej utworzone dokumenty wydania po numerze dokumentu z poziomu listy.
10. Użytkownik może otworzyć/podejrzeć szczegóły wcześniej zapisanego dokumentu wydania, w tym ponownie wygenerować/pobrać jego PDF.

## Kryteria akceptacji

- Da się od początku do końca utworzyć nowy dokument wydania: wybrać odbiorcę, dodać co najmniej jedną pozycję sprzętu, zapisać dokument — bez błędów.
- Zapisany dokument pojawia się na liście dokumentów wydania i jest odnajdywalny przez wyszukiwanie po numerze dokumentu.
- Dla zapisanego dokumentu można wygenerować plik PDF zawierający kompletne dane nagłówkowe dokumentu oraz pełną listę wydanych pozycji.
- Nie da się zapisać dokumentu wydania bez wskazania odbiorcy (pracownika i/lub lokalizacji) ani bez żadnej pozycji sprzętu.
- Sprzęt raz wydany na aktywnym dokumencie jest w jasny sposób odróżnialny od sprzętu dostępnego do wydania (np. nie pojawia się ponownie jako dostępny do wybrania na nowym dokumencie wydania), zgodnie z ustaleniami z etapu planowania.
- Na formularzu tworzenia dokumentu lista dostępnego sprzętu (z filtrami po producencie/typie/modelu/numerze seryjnym) i lista wybranych pozycji są widoczne jednocześnie, obok siebie, bez konieczności przewijania między nimi.

## Otwarte pytania

- Czy dokument wydania ma referować konkretne, fizyczne egzemplarze sprzętu (po numerze seryjnym), czy tylko typy/kategorie sprzętu i ilości?
- Jaka jest dokładna reguła numeracji dokumentów wydania (np. rok/miesiąc/kolejny numer) i czy numeracja musi być ciągła/bez luk?
- Jakie jest znaczenie i docelowy cykl życia flagi "zaksięgowano" (posted) — czy to krok zatwierdzenia dokumentu, po którym staje się on niemodyfikowalny? Kto może "zaksięgować" dokument?
- Czy dokument wydania zawsze wymaga wskazania zarówno strony wydającej (lokalizacja/pracownik źródłowy), jak i odbierającej, czy strona wydająca może pozostać pusta/domyślna?
- Jaki dokładnie układ/treść ma mieć wydruk PDF (np. miejsce na podpisy, dane firmy, logo)? Czy istnieje wzór dokumentu papierowego używanego dotychczas w firmie, który należałoby odwzorować?
- Czy wydanie sprzętu na tym dokumencie ma automatycznie aktualizować status/lokalizację/przypisanie danego sprzętu w ewidencji (`Equipment`/`EquipmentAssignment`), czy to osobny, ręczny krok?
- Czy zapisany dokument wydania (i jego pozycje) może być edytowany lub usunięty po zapisie, czy tylko odczytywany?

## odpowiedzi do pytań
- ma reffereować do konkretnego egzemplarza sprzętu - id / numer seryjny
- rok/miesiac/kolejny numer wydania
- dokument do którego załączono podpisany przez użytkownika plik pdf.
- strona wydająca - zalogowany użytkownik
- dokument w folderze AI-Data\handover\DRUK_FIRMOWY.pdf - zawiera datę listę urządzeń (tabela- producent / typ / model/ numer seryjny ) kto sporządził / dla kogo / miejsca na podpisy.
- tak aktualizacja danych w ewidencji
- wszystkie dokumenty wydania mają się znaleźć w bazie w tabelach!!!

---
*Uwaga: w repozytorium nie istnieje plik `_specs/template.md`, więc powyższa specyfikacja została przygotowana wg standardowej, uniwersalnej struktury dokumentu funkcjonalnego (Overview / Motywacja / Cele / Non-Goals / Użytkownicy / Obecny stan / Wymagania / Kryteria akceptacji / Otwarte pytania), tak jak przy poprzedniej specyfikacji w tym repozytorium.*
