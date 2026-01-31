# Outpost Immboile

## Opis projektu
System obsługujący maczkopaty

## Źródło adresów:

https://overpass-turbo.eu/

querka do ich pobrania:

```
[out:csv(::lat, ::lon, "addr:city", "addr:street", "addr:housenumber", "addr:postcode"; true; ",")];

(
  area["name"="Warszawa"]["admin_level"="8"];
  area["name"="Kraków"]["admin_level"="8"];
  area["name"="Wrocław"]["admin_level"="8"];
  area["name"="Gdańsk"]["admin_level"="8"];
  area["name"="Poznań"]["admin_level"="8"];
)->.searchArea;

(
  node["addr:housenumber"]["addr:street"](area.searchArea);
);

out qt 10000;
```

## Obsługa Dockera:

Ważne jest odpalenie
```
git lfs pull
```
Żeby pobrał się plik pbf z mapą

Pierwsze odpalenie z docker-compose za pomocą komendy:
```
sudo docker compose --profile importer up --build
```

Potem normalnie:
```
sudo docker compose up -d
```

## Styl PR:
[ "Czego temat dotyczy" ] #"numer zadania z issues" "Co się tutaj dzieje"

Przykład: [Infrastrucure] #1 Add Cqrs example and change readme

Main jest protected i ma zrobione readonly na pushe więc cokolwiek robimy to wrzucamy ładnie do PR :smile
Dopiero kiedy CI/CD przejdzie oraz któryś znasz puści to po CR, to bardzo ładnie robimy squash and merge.

W momencie kiedy są jakieś konflikty, to proszę nie robić force pushy, tylko ładnie domerdżować maina, rozwiązać konflikty i dopiero merdżować.

## Dokumentacja do MUI

https://mui.com/material-ui/getting-started/

## Testowanie
W route wejść w endpoint ```/scalar```

## Dodawanie rzeczy do schematu:
Dokumentacja do OpenApi-Ts:
https://openapi-ts.dev/openapi-react-query/use-query

W terminalu w folderze `outpostImmobile.app` wpisac ładnie 
```
npx openapi-typescript /home/kollibroman/LosoweProjekty/Outpost-Immobile/Applications/OutpostImmobile.Api/out/OutpostImmobile.Api.json -o ./src/schema.d.ts
```

## Skryptowanie migracji:

```
    dotnet ef migrations script --startup-project ./Outpost-Immobile.Api/Outpost-Immboile.Api.csproj --project ./Outpost-Immobile.Persistence/Outpost-Immboile.Persistence.csproj
```

## Odpalanie Fitnesse

```bash
docker compose --profile fitnesse up -d --build
```

Po uruchomieniu testy są dostępne pod adresem: http://localhost:9090

### Struktura testów akceptacyjnych

| Przypadek Użycia | Opis | URL |
|------------------|------|-----|
| PU5 | Odbiór paczek z maczkopatu | http://localhost:9090/FrontPage.PU5 |
| PU8 | Zawożenie paczek do magazynu | http://localhost:9090/FrontPage.PU8 |
| PU12 | Wysłanie powiadomienia | http://localhost:9090/FrontPage.PU12 |
| PU16 | Wydawanie paczek | http://localhost:9090/FrontPage.PU16 |
| PU17 | Aktualizacja stanu maczkopatu | http://localhost:9090/FrontPage.PU17 |

### Uruchamianie testów

1. **Wszystkie testy**: http://localhost:9090/FrontPage.TestPage → kliknij przycisk **Suite**
2. **Pojedynczy przypadek użycia**: Przejdź do wybranego PU i kliknij **Test**

### Wymagania przed uruchomieniem testów

Przed uruchomieniem FitNesse należy zbudować projekt testów akceptacyjnych:

```bash
dotnet build Core/OutpostImmobile.Core.Tests.Acceptance/OutpostImmobile.Core.Tests.Acceptance.csproj -c Release
```

### Restart FitNesse po zmianach w kodzie

Po wprowadzeniu zmian w fixture'ach należy:

1. Przebudować projekt:
```bash
dotnet build Core/OutpostImmobile.Core.Tests.Acceptance/OutpostImmobile.Core.Tests.Acceptance.csproj -c Release
```

2. Zrestartować kontener FitNesse:
```bash
docker compose --profile fitnesse restart fitnesse
```

### Uruchamianie testów z linii poleceń

```bash
# Uruchom wszystkie testy i pobierz wyniki w formacie tekstowym
curl "http://localhost:9090/FrontPage.TestPage?suite&format=text"

# Uruchom pojedynczy test
curl "http://localhost:9090/FrontPage.PU5?test&format=text"
```

### Zatrzymanie FitNesse

```bash
docker compose --profile fitnesse down
```

## Piszemy w CQRS, dokumentacja do DispatchR:
 https://github.com/hasanxdev/DispatchR
