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

## Piszemy w CQRS, dokumentacja do DispatchR:
 https://github.com/hasanxdev/DispatchR
