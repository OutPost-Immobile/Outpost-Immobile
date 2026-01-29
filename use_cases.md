# Test akceptacyjny: PU07. Odbiór paczek z magazynu
9.2 Metryka Przypadku Użycia
– Identyfikator: PU07
– Nazwa: Odbiór paczek z magazynu (wraz z powiązanymi przypadkami użycia:
PU09. Dostarczanie paczek do maczkopatu, PU15. Przyjmowanie paczek, PU17.
Aktualizacja stanu maczkopatu, PU12. Wysłanie powiadomienia)
– Aktor: Kurier
– Cel testu: Weryfikacja poprawności procesu załadowania paczek z magazynu do
maczkopatu oraz aktualizacji dostępności skrytek.
– Spodziewany wynik: Rejestracja przesyłek w docelowym urządzeniu, zmiana sta-
tusów w bazie danych, wysłanie klientom powiadomień oraz skuteczne zablokowanie
działań nieautoryzowanego aktora.
9.2.1 Przebieg Scenariusza (Kroki)
Wszystkie działania aktora są wykonywane ręcznie.
1. Autoryzacja: Kurier klika "Zaloguj"na stronie głównej, wprowadza email oraz
hasło, a następnie zatwierdza przyciskiem "Zaloguj".
• System nadaje sesję kuriera i wyświetla menu główne z dostępem do operacji
logistycznych.
2. Wybór urządzenia: Kurier klika "Stan maczkopatu"i wprowadza unikalne ID
maczkopatu, do którego dostarcza przesyłki (inicjacja PU09. Dostarczanie pa-
czek do maczkopatu).
• System wyświetla widok zawartości maczkopatu, prezentując listę przypisa-
nych do niego paczek.
3. Inicjacja PU9. Dostarczanie paczek do maczkopatu: Kurier wybiera opcję
aktualizacji statusu paczek i wpisuje kody przesyłek pobranych z magazynu.
• Status przesyłek w bazie danych zmienia się na "W tranzycie".
4. Inicjacja PU15. Przyjmowanie paczek: Kurier wybiera opcję aktualizacji sta-
tusu paczek i wpisuje kody przesyłek które ma włożyć do maczkopatu (inicjacja
PU17. Aktualizacja stanu maczkopatu).
• Status przesyłek w bazie danych zmienia się na "W maczkopacie".
5. Inicjacja PU17. Aktualizacja stanu maczkopatu: Kurier wybiera opcję "Zmień
status"na stronie aktualizacji statusu paczek przy umieszczaniu każdej przesyłki w
maczkopacie (inicjacja PU12. Wysłanie powiadomienia).
• Stan maczkopatu jest aktualizowany (zwiekszany).
70
6. Inicjacja PU12. Wysłanie powiadomienia: System automatycznie generuje
powiadomienia do klientów o zmianie statusu paczki i wysyła je mailem.
• Klienci otrzymują powiadomienia o zmianie statusu paczki na "W maczkopa-
cie".
Wynik:
Poprawna rejestracja przesyłek w docelowym urządzeniu, zmiana statusów w bazie da-
nych, wysłanie klientom powiadomień oraz skuteczne zablokowanie działań nieautoryzo-
wanego aktora.
# 9.3 Test akceptacyjny: PU08. Zawożenie paczek do magazynu
– Nazwa: Zawożenie paczek do magazynu (wraz z powiązanymi przypadkami użycia:
PU05. Odbiór paczek z maczkopatu, PU16. Wydawanie paczek, PU17. Aktualizacja
stanu maczkopatu, PU12. Wysłanie powiadomienia)
– Aktor: Kurier
– Cel testu: Weryfikacja poprawności procesu opróżniania skrytek maczkopatu i
aktualizacji jego stanu.
– Wynik spodziewany: Pełna synchronizacja stanu faktycznego (stanu maczko-
patów) ze stanem bazodanowym, wysłanie klientom powiadomień oraz poprawna
aktualizacja statusów paczek.
9.3.1 Przebieg Scenariusza (Kroki)
Wszystkie działania aktora są wykonywane ręcznie.
1. Autoryzacja: Kurier klika "Zaloguj"na stronie głównej, wprowadza email oraz
hasło, a następnie zatwierdza przyciskiem "Zaloguj".
• System nadaje sesję z uprawnieniami kuriera i przekierowuje do panelu wyboru
urządzenia.
2. Wybór urządzenia: Kurier klika "Stan maczkopatu"i wprowadza unikalne ID
maczkopatu w polu wyszukiwania na widoku zawartości maczkopatu (inicjacja
PU05. Odbiór paczek z maczkopatu).
• System wyświetla listę wszystkich paczek przypisanych do maczkopatu (za-
równo do odbioru przez klientów, jak i do zawiezienia do magazynu).
3. Inicjacja PU05. Odbiór paczek z maczkopatu: Kurier po zalogowaniu się wy-
biera opcję aktualizacji statusu paczek i wpisuje kody przesyłek, które ma odebrać
z maczkopatu (inicjacja PU16. Wydawanie paczek).
• Kurier odbiera paczki z maczkopatu.
4. Inicjacja PU16. Wydawanie paczek: Kurier wybiera paczki przeznaczone do
zawiezienia do magazynu i otwiera przypisane do nich skrytki (inicjacja PU17.
Aktualizacja stanu maczkopatu).
71
• Maczkopat otwiera odpowiednie skrytki, a po ich opróżnieniu status przesyłek
jest zmieniony przez kuriera na „Wysłane do magazynu”.
5. Inicjacja PU17. Aktualizacja stanu maczkopatu: Kurier wybiera opcję "Zmień
status"na stronie aktualizacji statusu paczek przy umieszczaniu każdej przesyłki w
maczkopacie (inicjacja PU12. Wysłanie powiadomienia).
• Stan maczkopatu jest aktualizowany (zmniejszany).
6. Inicjacja PU12. Wysłanie powiadomienia: System automatycznie generuje
powiadomienia do klientów o zmianie statusu paczki i wysyła je mailem.
• Klienci otrzymują powiadomienia o zmianie statusu paczki na "Wysłane do
magazynu".
Wynik
Pełna synchronizacja stanu faktycznego (puste skrytki) ze stanem bazodanowym, wysła-
nie klientom powiadomień oraz poprawna aktualizacja statusów paczek.