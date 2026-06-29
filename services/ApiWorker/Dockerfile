FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /app

RUN dotnet tool install --global dotnet-ef --version 10.0.8
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ["ApiWorker.csproj", "ApiWorker.csproj"]

RUN dotnet restore --no-cache
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION

FROM build AS migrate
WORKDIR /app
ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build", "--project", "ApiWorker.csproj"]

FROM build AS publish
WORKDIR /app
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApiWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiWorker.dll"]
