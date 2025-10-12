FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OutpostImmobile.Api/OutpostImmobile.Api.csproj", "OutpostImmobile.Api/"]
COPY ["OutpostImmobile.Core/OutpostImmobile.Core.csproj", "OutpostImmobile.Core/"]
COPY ["OutpostImmobile.Persistence/OutpostImmobile.Persistence.csproj", "OutpostImmobile.Persistence/"]
RUN dotnet restore "OutpostImmobile.Api/OutpostImmobile.Api.csproj"
COPY . .
WORKDIR "/src/OutpostImmobile.Api"
RUN dotnet build "./OutpostImmobile.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OutpostImmobile.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OutpostImmobile.Api.dll"]
