# Uprawnienia: Użytkownik (podgląd) / Administrator (modyfikacje)

## Cel

Wprowadzić dwa poziomy dostępu do aplikacji AssetSquirrel oparte o istniejące tabele ASP.NET Identity (`AspNetRoles`, `AspNetUserRoles`):

1. **Użytkownik** — może przeglądać wszystkie dane (sprzęt, pracownicy, wydania/zwroty, faktury, lokalizacje, słowniki, przypisania sprzętu), ale nie może niczego dodawać, edytować ani usuwać.
2. **Administrator** — ma pełny dostęp, tak jak obecnie: dodawanie, edycja i usuwanie we wszystkich modułach.

## Kontekst / stan obecny

- Identity jest skonfigurowane przez `AddIdentityCore<ApplicationUser>` (bez `.AddRoles<IdentityRole>()`) — role nie są dziś w ogóle używane w aplikacji, mimo że tabele `AspNetRoles` / `AspNetUserRoles` / `AspNetRoleClaims` już istnieją w bazie (jako część `IdentityDbContext<ApplicationUser>`).
- Jedyna dzisiejsza kontrola dostępu to wymóg zalogowania (`AuthorizeRouteView` w `Routes.razor`) — każdy zalogowany użytkownik ma dziś pełne prawa modyfikacji.
- Nie istnieje żadna strona do zarządzania użytkownikami/rolami — trzeba ją będzie stworzyć.
- Nie ma seeda roli/administratora — pierwsze konto administratora trzeba będzie jakoś ustawić.
- Moduły z akcjami modyfikującymi, które trzeba objąć ograniczeniem dla roli Użytkownik: Employees, Equipment, EquipmentHandover, EquipmentReturn, Invoices, Locations, Dictionares (HardwareType, Manufacturer, Suppilers, Equipment, Internet), EquipmentAssignment.

## Zakres (co wchodzi)

- Dodanie obsługi ról ASP.NET Identity (`IdentityRole`, `RoleManager`) do istniejącej konfiguracji Identity.
- Zdefiniowanie dwóch ról: **Admin** i **User** (dokładne nazwy do potwierdzenia).
- Zablokowanie w całej aplikacji (UI + logika) akcji dodawania/edycji/usuwania dla roli User — użytkownik z tą rolą widzi te same listy i szczegóły co dziś, ale przyciski/akcje modyfikujące są niedostępne (ukryte lub wyłączone) i niedostępne obejściem (np. bezpośrednie odpalenie use case'a musi też być zablokowane, nie tylko UI).
- Mechanizm nadawania roli kontu — na start wystarczy, że da się to zrobić ręcznie (np. bezpośrednio w bazie / prosty ekran administracyjny) — do ustalenia w sekcji Pytania otwarte.
- Domyślne zachowanie dla nowo zakładanych kont (np. przez rejestrację, jeśli jest włączona) — do ustalenia.

## Poza zakresem (co nie wchodzi)

- Bardziej granularne uprawnienia (np. różne prawa per moduł, per lokalizacja) — na razie tylko dwa poziomy: podgląd / pełne modyfikacje.
- Zarządzanie zaproszeniami, self-service podnoszenie uprawnień przez użytkownika.
- Audyt/logowanie kto i kiedy zmienił dane (chyba że już istnieje — do potwierdzenia, nie jest to przedmiotem tej funkcji).

## Historyjki użytkownika

- Jako **użytkownik** chcę móc przeglądać sprzęt, pracowników, wydania/zwroty, faktury, lokalizacje i słowniki, ale nie chcę mieć możliwości przypadkowego dodania/edycji/usunięcia danych.
- Jako **administrator** chcę zachować pełną możliwość zarządzania wszystkimi danymi, tak jak działa to dzisiaj.
- Jako **administrator** chcę mieć sposób na nadanie komuś roli Użytkownik lub Administrator.

## Wymagania funkcjonalne

1. System ról Identity musi zostać włączony (rola Admin i User dostępne w `AspNetRoles`).
2. Każda akcja dodawania/edycji/usuwania w każdym module musi być dostępna wyłącznie dla roli Admin — zarówno na poziomie UI (przyciski/dialogi niewidoczne lub wyłączone dla User), jak i na poziomie autoryzacji use case'a/serwera (żeby nie dało się obejść przez samo ukrycie przycisku).
3. Rola User musi mieć pełny dostęp do wszystkich widoków/list/szczegółów w takim samym zakresie jak dziś (żadne dane nie znikają, znika tylko możliwość modyfikacji).
4. Musi istnieć sposób przypisania roli do konta użytkownika (zakres do ustalenia — patrz Pytania otwarte).
5. Musi być zdefiniowane zachowanie domyślne: jaką rolę dostaje nowo utworzone/zarejestrowane konto, zanim ktoś ręcznie nada mu rolę Admin.
6. Próba wejścia użytkownika z rolą User na akcję zarezerwowaną dla Admina (np. przez bezpośredni URL edycji, jeśli taki istnieje) musi być bezpiecznie blokowana, a nie powodować błąd aplikacji.

## Kryteria sukcesu

- Konto z rolą User może przeglądać wszystkie moduły, ale nie widzi/nie może użyć żadnej akcji dodawania, edycji ani usuwania.
- Konto z rolą Admin działa dokładnie tak jak obecne konta dzisiaj (bez regresji).
- Próba wywołania akcji modyfikującej przez konto User (nawet z pominięciem UI) kończy się odmową dostępu, a nie wykonaniem operacji.
- Istnieje udokumentowany/powtarzalny sposób nadania roli Admin przynajmniej jednemu kontu (żeby dało się w ogóle zacząć zarządzać rolami po wdrożeniu).

## Pytania otwarte

1. Czy potrzebny jest pełny ekran administracyjny do zarządzania użytkownikami i rolami (lista użytkowników + zmiana roli z UI), czy na start wystarczy nadawanie roli ręcznie (np. skryptem/SQL/seedem) i temat UI odłożyć na później?
2. Jak nazwać role — dokładnie "Admin"/"User", czy inne nazwy (np. po polsku "Administrator"/"Użytkownik")?
3. Jaką rolę powinno domyślnie dostawać nowe konto zakładane przez rejestrację (jeśli rejestracja jest w ogóle używana w praktyce) — User, czy nowe konta mają wymagać ręcznej aktywacji/nadania roli zanim będą mogły cokolwiek robić?
4. Co z istniejącymi już kontami w bazie — czy wszystkie mają dostać rolę Admin przy wdrożeniu (zachowanie stanu obecnego), czy część ma dostać User?
5. Czy commit "Napraw czytelnosc pol readonly" (a55c605) i inne niedawne zmiany UI pod tryb tylko-do-odczytu mają zostać wykorzystane/rozszerzone w ramach tej funkcji, czy to osobny wątek?

1. Widok z listą wszystkich użytkowników / możliwością blokowania kont / zmiany uprawnień / edycji danych np. imię i nazwisko.
2. Admin ? View
3. View - tylko wgląd.
4. Admin - zresetuj hasło - ustaw 11111111
5. Mają zostać wykorzystane - nie zgub żadnych zmian - przed startemmodyfikacji commit i merge z masterem.
