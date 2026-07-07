# Sci-Fi Inspired App Layout — Light & Dark Themes

## Overview

Nowy, całościowy layout wizualny aplikacji AssetSquirrel (Blazor Server), dostępny w dwóch wariantach kolorystycznych — jasnym i ciemnym — utrzymany w bardzo nowoczesnej, futurystycznej estetyce inspirowanej klimatem "Blade Runner", Star Trek i Gwiezdnych Wojen.

## Problem / Motywacja

Obecny UI aplikacji opiera się na standardowym, niezmodyfikowanym Bootstrapie (tabele, przyciski, formularze w domyślnym, "korporacyjnym" stylu). Nie ma żadnej spójnej tożsamości wizualnej ani trybu ciemnego. Wygląd nie odróżnia się od typowych, generycznych aplikacji administracyjnych.

## Cele

- Zaprojektować i wdrożyć nowy, spójny system wizualny dla całej aplikacji (layout, kolorystyka, typografia, komponenty: tabele, przyciski, formularze, modale/dialogi, pasek nawigacji).
- Dostarczyć dwa pełnoprawne warianty motywu: **jasny** i **ciemny**, z możliwością przełączania przez użytkownika.
- Estetyka ma nawiązywać do futurystycznego, "sci-fi" klimatu (Blade Runner / Star Trek / Star Wars) — np. neonowe akcenty, wyraziste kontrasty, nowoczesna typografia, subtelne efekty świetlne/gradienty, motywy "HUD"/interfejsu statku kosmicznego — bez utraty czytelności i użyteczności aplikacji biznesowej.
- Zachować spójność wizualną we wszystkich istniejących obszarach aplikacji (Equipment, Employees, Invoices, Locations, Dictionares, EquipmentHandover, strony logowania/Identity).
- Wybrany motyw użytkownika powinien być zapamiętywany między sesjami.

## Poza zakresem (Non-Goals)

- Zmiana logiki biznesowej, przepływów danych lub struktury nawigacji aplikacji.
- Redesign treści/informacji prezentowanych na poszczególnych stronach (liczba kolumn w tabelach, pola formularzy itp.) — zmiana dotyczy wyłącznie warstwy wizualnej.
- Wsparcie dla dodatkowych motywów kolorystycznych poza jasnym i ciemnym (na tym etapie).
- Zmiany w responsywności/wersji mobilnej wykraczające poza to, co już istnieje (chyba że nowy layout tego wymaga).

## Użytkownicy i przypadki użycia

- Pracownik działu IT korzystający z aplikacji na co dzień do ewidencji sprzętu — oczekuje przyjemnego, nowoczesnego interfejsu bez utraty szybkości pracy.
- Użytkownik pracujący wieczorem/w słabo oświetlonym pomieszczeniu — korzysta z trybu ciemnego, żeby ograniczyć zmęczenie oczu.
- Nowy użytkownik/gość prezentujący aplikację — pierwsze wrażenie wizualne ma być nowoczesne i profesjonalne.

## Kierunek wizualny / Design Direction

- **Paleta jasna**: czyste, jasne tło z wyrazistymi akcentami kolorystycznymi (np. cyjan/niebieski, ewentualnie bursztynowy/pomarańczowy jako akcent), wysoki kontrast tekstu.
- **Paleta ciemna**: głębokie, prawie czarne/granatowe tło, neonowe akcenty (cyjan, fiolet, bursztyn), efekt "interfejsu statku kosmicznego" — subtelne poświaty (glow), ostre linie/obramowania, ewentualnie delikatne animacje przejść.
- Nowoczesna typografia (np. font o geometrycznym, technicznym charakterze dla nagłówków).
- Spójne ikony, przyciski i komponenty modalne dopasowane do obu wariantów.
- Przełącznik motywu (jasny/ciemny) widoczny i łatwo dostępny w interfejsie (np. w pasku nawigacji).

## Wymagania funkcjonalne

1. Użytkownik może przełączyć motyw aplikacji między jasnym a ciemnym z poziomu UI.
2. Wybrany motyw jest zapamiętywany (np. w local storage przeglądarki lub w preferencjach użytkownika) i przywracany przy kolejnej wizycie.
3. Wszystkie istniejące strony i komponenty (tabele, formularze, dialogi/modale, strony logowania) poprawnie renderują się w obu motywach — bez utraty czytelności czy funkcjonalności.
4. Elementy interaktywne (przyciski, linki, pola formularzy, komunikaty sukcesu/błędu z iziToast) zachowują odpowiedni kontrast i czytelność w obu wariantach.
5. Nowy layout nie zmienia adresów URL ani struktury nawigacji między stronami.

## Kryteria akceptacji

- Aplikacja oferuje przełącznik motywu dostępny z każdej strony.
- Po przełączeniu motywu cały interfejs (nie tylko wybrane fragmenty) zmienia wygląd zgodnie z nową paletą.
- Wybór motywu przetrwa odświeżenie strony i ponowne otwarcie aplikacji.
- Wygląd obu motywów jest spójny wizualnie ze sobą (ten sam layout, inna paleta) i nawiązuje do opisanego klimatu sci-fi.
- Żadna z dotychczasowych funkcji aplikacji (dodawanie/edycja/usuwanie sprzętu, pracowników, faktur itd.) nie przestaje działać po wdrożeniu nowego layoutu.

## Otwarte pytania

- Czy przełącznik motywu ma być globalny dla wszystkich użytkowników, czy indywidualny per użytkownik (zapisany w profilu w bazie), czy tylko lokalny w przeglądarce?
- Czy potrzebny jest konkretny zestaw kolorów/wytycznych brandingowych firmy, czy pełna dowolność w ramach opisanego klimatu sci-fi?
- Czy strony logowania/rejestracji (Identity) też mają zostać objęte nowym layoutem, czy zostają w obecnym stylu?
- Czy istnieją ograniczenia dot. bibliotek/frameworków CSS (obecnie: Bootstrap 5 + bootstrap-icons + iziToast) — czy można je zastąpić/rozszerzyć, czy trzeba pozostać w ich ramach?

---
*Uwaga: w repozytorium nie istniał plik `_specs/template.md`, więc powyższa specyfikacja została przygotowana wg standardowej, uniwersalnej struktury dokumentu funkcjonalnego (Overview / Motywacja / Cele / Non-Goals / Użytkownicy / Design Direction / Wymagania / Kryteria akceptacji / Otwarte pytania).*
