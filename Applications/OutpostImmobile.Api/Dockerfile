FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Applications/OutpostImmobile.Api/OutpostImmobile.Api.csproj", "Applications/OutpostImmobile.Api/"]
COPY ["Core/OutpostImmobile.Core.Common/OutpostImmobile.Core.Common.csproj", "Core/OutpostImmobile.Core.Common/"]
COPY ["Persistence/OutpostImmobile.Persistence/OutpostImmobile.Persistence.csproj", "Persistence/OutpostImmobile.Persistence/"]
COPY ["Core/OutpostImmobile.Core.Mediator/OutpostImmobile.Core.Mediator.csproj", "Core/OutpostImmobile.Core.Mediator/"]
COPY ["Core/OutpostImmobile.Core/OutpostImmobile.Core.csproj", "Core/OutpostImmobile.Core/"]
COPY ["Core/OutpostImmobile.Communication/OutpostImmobile.Communication.csproj", "Core/OutpostImmobile.Communication/"]
COPY ["Core/OutpostImmobile.Core.Integrations.KMZB/OutpostImmobile.Core.Integrations.KMZB.csproj", "Core/OutpostImmobile.Core.Integrations.KMZB/"]
RUN dotnet restore "Applications/OutpostImmobile.Api/OutpostImmobile.Api.csproj"
COPY . .
WORKDIR "/src/Applications/OutpostImmobile.Api"
RUN dotnet build "./OutpostImmobile.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OutpostImmobile.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OutpostImmobile.Api.dll"]
